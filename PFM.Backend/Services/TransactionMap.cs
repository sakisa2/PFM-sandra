using CsvHelper.Configuration;
using PFM.Models;

public sealed class TransactionMap : ClassMap<Transaction>
{
    public TransactionMap()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.BeneficiaryName).Name("beneficiary-name");
        Map(m => m.Date).Name("date").Convert(row =>
        {
            var date = DateTime.Parse(row.Row.GetField("date"));
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        });        
        Map(m => m.Direction).Name("direction");
        Map(m => m.Amount).Name("amount");
        Map(m => m.Description).Name("description");
        Map(m => m.Currency).Name("currency");
        Map(m => m.Mcc).Name("mcc");
        Map(m => m.Kind).Name("kind");
    }
}
