using System.ComponentModel.DataAnnotations;

namespace PFM.Backend.Database.Entities
{
    public class CreateTransactionDTO
    {

        [Required]
        public string Id { get; set; }

        public string BeneficiaryName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Direction Direction { get; set; }

        [Required]
        public double Amount { get; set; }

        public string Description { get; set; }

        [Required]
        [RegularExpression("[A-Za-z]{3}", ErrorMessage = "Currency must be from ISO 4271")]
        public string Currency { get; set; }

        [RegularExpression("([0-9]{4}$)?", ErrorMessage = "Mcc code must be 4 digit code")]
        public string Mcc { get; set; }

        [Required]
        public Kind Kind { get; set; }
    }

}
