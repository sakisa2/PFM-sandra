using System.Collections.Generic;

namespace PFM.Backend.Models.Responses
{
    public class SpendingGroupDTO
    {
        public string Category { get; set; }
        public double TotalAmount { get; set; }

        public List<SpendingSubcategoryDTO> Subcategories { get; set; } = new();
    }
}

