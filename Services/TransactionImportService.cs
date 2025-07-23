using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PFM.Data;
using System.Globalization;
using PFM.Backend.Database.Entities;
using PFM.Backend.Models.Validation;

namespace PFM.Backend.Services;

public class TransactionImportService
{
    private readonly TransactionDbContext _dbContext;

    public TransactionImportService(TransactionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ValidationResult> ImportTransactionsValidatedAsync(Stream csvStream)
    {
        var result = new ValidationResult();
        var validTransactions = new List<Transaction>();

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        csv.Context.RegisterClassMap<TransactionMap>();

        List<Transaction> records;
        try
        {
            records = csv.GetRecords<Transaction>().ToList();
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ValidationError("CSV", "parse-error", $"Failed to parse CSV: {ex.Message}"));
            return result;
        }

        foreach (var record in records)
        {
            var errors = ValidateTransaction(record);
            if (errors.Any())
            {
                result.Errors.AddRange(errors);
            }
            else
            {
                validTransactions.Add(record);
            }
        }

        if (result.IsValid)
        {
            try
            {
                await _dbContext.Transactions.AddRangeAsync(validTransactions);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationError("Database", "save-error", $"Error while saving to DB: {ex.Message}"));
            }
        }

        return result;
    }

    private List<ValidationError> ValidateTransaction(Transaction transaction)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(transaction.Id))
            errors.Add(new ValidationError(nameof(transaction.Id), "id-required", "Transaction ID is required."));

        if (transaction.Date == default)
            errors.Add(new ValidationError(nameof(transaction.Date), "date-required", "Transaction date is required."));

        if (!Enum.IsDefined(typeof(Direction), transaction.Direction))
            errors.Add(new ValidationError(nameof(transaction.Direction), "invalid-direction", $"Invalid direction: {transaction.Direction}"));

        if (transaction.Amount <= 0)
            errors.Add(new ValidationError(nameof(transaction.Amount), "invalid-amount", "Amount must be greater than zero."));

        if (string.IsNullOrWhiteSpace(transaction.Currency))
            errors.Add(new ValidationError(nameof(transaction.Currency), "currency-required", "Currency is required."));

        if (!Enum.IsDefined(typeof(Kind), transaction.Kind))
            errors.Add(new ValidationError(nameof(transaction.Kind), "invalid-kind", $"Invalid kind: {transaction.Kind}"));

        return errors;
    }
}
