using Microsoft.EntityFrameworkCore;
using PFM.Backend.Database.Entities;
using PFM.Backend.Models.Queries;
using PFM.Backend.Models.Requests;
using PFM.Backend.Models.Responses;
using PFM.Data;
using AutoMapper;
using PFM.Backend.Models.Exceptions;
using PFM.Backend.Models.Validation;


namespace PFM.Backend.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IMapper _mapper;

        private readonly TransactionDbContext _context;

        public TransactionService(TransactionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CategorizeTransactionAsync(string transactionId, string categoryCode)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null) return false;

            var category = await _context.Categories.FindAsync(categoryCode);
            if (category == null) return false;

            transaction.Catcode = categoryCode;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<TransactionDTO>> GetTransactionsAsync(TransactionQueryParameters parameters)
        {
            var query = _context.Transactions.AsQueryable();

            if (parameters.StartDate.HasValue)
            {
                var startUtc = DateTime.SpecifyKind(parameters.StartDate.Value, DateTimeKind.Utc);
                query = query.Where(t => t.Date >= startUtc);
            }

            if (parameters.EndDate.HasValue)
            {
                var endUtc = DateTime.SpecifyKind(parameters.EndDate.Value, DateTimeKind.Utc);
                query = query.Where(t => t.Date <= endUtc);
            }

            if (parameters.Kinds != null && parameters.Kinds.Any())
            {
                query = query.Where(t => parameters.Kinds.Contains(t.Kind));
            }

            var validationResult = new ValidationResult();

            if (parameters.Page < 1)
                validationResult.AddError("page", "min-value", "Page number must be greater than or equal to 1");

            if (parameters.PageSize > 100)
                validationResult.AddError("page-size", "max-value", "Page size must not exceed 100");

            if (parameters.PageSize < 1)
                validationResult.AddError("page-size", "min-value", "Page size must be greater than 0");

            if (!string.IsNullOrEmpty(parameters.SortOrder) &&
                parameters.SortOrder.ToLower() != "asc" && parameters.SortOrder.ToLower() != "desc")
            {
                validationResult.AddError("sort-order", "invalid-value", "Sort order must be either 'asc' or 'desc'");
            }

            if (!string.IsNullOrEmpty(parameters.SortBy) &&
                parameters.SortBy.ToLower() != "date" && parameters.SortBy.ToLower() != "amount")
            {
                validationResult.AddError("sort-by", "invalid-field", "SortBy must be 'date' or 'amount'");
            }

            if (validationResult.Errors.Any())
                throw new ValidationException(validationResult);



            switch (parameters.SortBy?.ToLower())
            {
                case "amount":
                    query = parameters.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(t => t.Amount)
                        : query.OrderBy(t => t.Amount);
                    break;

                case "date":
                    
                    query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.Date)
                            : query.OrderBy(t => t.Date);
                    break;
                default:
                    query = query.OrderBy(t => t.Date);

                    break;
            }


            var totalCount = await query.CountAsync();

            var entities = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

            var dtos = _mapper.Map<List<TransactionDTO>>(entities);

            return new PagedResult<TransactionDTO>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize,
                SortBy = parameters.SortBy,
                SortOrder = parameters.SortOrder
            };
        }
        public async Task<SpendingAnalyticsResponse> GetSpendingAnalyticsAsync(
            string? catcode = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            Direction? direction = null)
        {
            var query = _context.Transactions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(catcode))
                query = query.Where(t => t.Catcode != null && t.Catcode.StartsWith(catcode));

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            if (direction.HasValue)
                query = query.Where(t => t.Direction == direction.ToString());

            var transactions = await query.ToListAsync();

            if (string.IsNullOrWhiteSpace(catcode))
            {
                // Grupisanje po top-level kategorijama
                var grouped = transactions
                    .GroupBy(t => t.Catcode?.Split('_')[0])
                    .Select(g => new SpendingGroupDTO
                    {
                        Category = g.Key,
                        TotalAmount = g.Sum(t => t.Amount),
                        Subcategories = g
                            .GroupBy(t => t.Catcode)
                            .Select(sub => new SpendingSubcategoryDTO
                            {
                                Category = sub.Key,
                                TotalAmount = sub.Sum(t => t.Amount)
                            })
                            .ToList()
                    })
                    .ToList();

                return new SpendingAnalyticsResponse
                {
                    Groups = grouped
                };
            }
            else
            {
                // Samo jedna grupa sa podkategorijama za dati catcode
                var group = new SpendingGroupDTO
                {
                    Category = catcode,
                    TotalAmount = transactions.Sum(t => t.Amount),
                    Subcategories = transactions
                        .GroupBy(t => t.Catcode)
                        .Select(g => new SpendingSubcategoryDTO
                        {
                            Category = g.Key,
                            TotalAmount = g.Sum(t => t.Amount)
                        })
                        .ToList()
                };

                return new SpendingAnalyticsResponse
                {
                    Groups = new List<SpendingGroupDTO> { group }
                };
            }
        }

        public async Task<bool> SplitTransactionAsync(string transactionId, List<SplitItemDTO> splits)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Splits)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
            {
                Console.WriteLine($"Transakcija sa ID = '{transactionId}' nije pronađena u bazi.");
                return false;
            }

            
            var existingSplits = _context.TransactionSplits.Where(s => s.TransactionId == transactionId);
            _context.TransactionSplits.RemoveRange(existingSplits);

            foreach (var split in splits)
            {
                var category = await _context.Categories.FindAsync(split.Catcode);

                if (category == null)
                {
                    Console.WriteLine($"Kategorija sa catcode = '{split.Catcode}' nije pronađena u bazi.");
                    return false;
                }

                var newSplit = new TransactionSplit
                {
                    TransactionId = transactionId,
                    Catcode = split.Catcode,
                    Amount = split.Amount
                };

                _context.TransactionSplits.Add(newSplit);
            }

            await _context.SaveChangesAsync();
            return true;
        }


    }

}
