using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PFM.Backend.Models.Requests
{
    public class SplitItemDTO
    {
        [Required]
        public string Catcode { get; set; }

        [Required]
        public double Amount { get; set; }
    }

    public class SplitTransactionDTO
    {
        [Required]
        public List<SplitItemDTO> Splits { get; set; }
    }
}
