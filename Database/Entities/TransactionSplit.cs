using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PFM.Backend.Database.Entities;

namespace PFM.Backend.Database.Entities
{
    public class TransactionSplit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TransactionId { get; set; }

        [Required]
        public string Catcode { get; set; }

        [Required]
        public double Amount { get; set; }

        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }
    }
}
