using PFM.Backend.Database.Entities;
using PFM.Backend.Models.Queries;
using PFM.Backend.Models.Responses;
using PFM.Backend.Models.Requests;
using PFM.Backend.Models;

namespace PFM.Backend.Services
{
    public interface ITransactionService
    {
        Task<PagedResult<TransactionDTO>> GetTransactionsAsync(TransactionQueryParameters parameters);
        Task<bool> CategorizeTransactionAsync(string transactionId, string categoryCode);

        Task<SpendingAnalyticsResponse> GetSpendingAnalyticsAsync(
        string? catcode,
        DateTime? startDate,
        DateTime? endDate,
        Direction? direction
    );
        Task<bool> SplitTransactionAsync(string transactionId, List<SplitItemDTO> splits);
        Task AutoCategorize();


    }
}

