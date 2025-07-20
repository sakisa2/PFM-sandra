using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PFM.Data;
using PFM.Models;
using System.Globalization;
using PFM.Backend.Database.Entities;

namespace PFM.Backend.Services;

public class TransactionImportService
{
    private readonly TransactionDbContext _dbContext;

    private List<Transaction> validTransactions = new List<Transaction>(); 
    
    public TransactionImportService(TransactionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ImportTransactionsAsync(Stream csvStream)
    {
        Console.WriteLine(">>> ImportTransactionsAsync START <<<");
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        csv.Context.RegisterClassMap<TransactionMap>(); // Registrujemo mapiranje

        var records = csv.GetRecords<Transaction>().ToList();


   foreach (var record in records)
{
    try
    {
        ValidateTransaction(record);
        validTransactions.Add(record);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Skipped record {record.Id}: {ex.Message}");
    }
}
try
{
    await _dbContext.Transactions.AddRangeAsync(validTransactions);
    await _dbContext.SaveChangesAsync();
}
catch (Exception ex)
{
    Console.WriteLine(">>> ERROR while saving to DB <<<");
    Console.WriteLine(ex.Message);
    if (ex.InnerException != null)
        Console.WriteLine("Inner Exception: " + ex.InnerException.Message);

    throw; 
}

        Console.WriteLine(">>> ImportTransactionsAsync END <<<");
    }

    private void ValidateTransaction(Transaction transaction)
{
    if (string.IsNullOrWhiteSpace(transaction.Id))
        throw new Exception("Transaction ID is required.");

    if (string.IsNullOrWhiteSpace(transaction.BeneficiaryName))
        throw new Exception("Beneficiary name is required.");

    if (transaction.Date == default)
        throw new Exception("Transaction date is required.");

    if (!Enum.IsDefined(typeof(Direction), transaction.Direction))
        throw new Exception($"Invalid direction: {transaction.Direction}");

    if (transaction.Amount <= 0)
        throw new Exception($"Invalid amount: {transaction.Amount}");

    if (string.IsNullOrWhiteSpace(transaction.Currency))
        throw new Exception("Currency is required.");

    if (string.IsNullOrWhiteSpace(transaction.Mcc))
        throw new Exception("MCC is required.");

    if (!Enum.IsDefined(typeof(Kind), transaction.Kind))
        throw new Exception("Transaction kind is required.");
}
}
