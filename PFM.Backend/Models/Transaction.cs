using System;
using PFM.Backend.Database.Entities; 

namespace PFM.Models
{
    public class Transaction
    {
        public string Id { get; set; }

        public string? BeneficiaryName { get; set; }

        public DateTime Date { get; set; }

        public Direction Direction { get; set; }

        public double Amount { get; set; }

        public string? Description { get; set; }

        public string Currency { get; set; }

        public string? Mcc { get; set; }

        public Kind Kind { get; set; }

        public override string ToString()
{
    return $"Id: {Id}, BeneficiaryName: {BeneficiaryName}, Date: {Date}, Direction: {Direction}, Amount: {Amount}, Description: {Description}, Currency: {Currency}, Mcc: {Mcc}, Kind: {Kind}";
}

    }
    
}
