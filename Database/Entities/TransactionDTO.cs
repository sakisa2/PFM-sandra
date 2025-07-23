using PFM.Backend.Database.Entities;

namespace PFM.Backend.Database.Entities
{
    public class TransactionDTO
    {
        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public Direction Direction { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public int Mcc { get; set; }
        public Kind Kind { get; set; }

    }
}