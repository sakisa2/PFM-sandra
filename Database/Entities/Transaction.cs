using System;
using PFM.Backend.Database.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PFM.Backend.Database.Entities
{
    public class Transaction
    {
        public ICollection<TransactionSplit> Splits { get; set; } = new List<TransactionSplit>();

        [Key]
        public string Id { get; set; }

        public string? BeneficiaryName { get; set; }

        public DateTime Date { get; set; }

        public string Direction { get; set; }

        public double Amount { get; set; }

        public string? Description { get; set; }

        public string Currency { get; set; }

        public string? Mcc { get; set; }

        public Kind Kind { get; set; }

        public string? Catcode { get; set; }

        [ForeignKey("Catcode")]
        public Category? Category { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, BeneficiaryName: {BeneficiaryName}, Date: {Date}, Direction: {Direction}, Amount: {Amount}, Description: {Description}, Currency: {Currency}, Mcc: {Mcc}, Kind: {Kind}";
        }

    }
    
}
