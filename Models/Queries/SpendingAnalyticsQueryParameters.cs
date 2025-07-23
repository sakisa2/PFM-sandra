using System;
using PFM.Backend.Database.Entities;

namespace PFM.Backend.Models.Queries
{
    public class SpendingAnalyticsQueryParameters
    {
        public string? Catcode { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Direction? Direction { get; set; } 
    }
}
