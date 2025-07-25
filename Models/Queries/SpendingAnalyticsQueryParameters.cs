using Microsoft.AspNetCore.Mvc;
using PFM.Backend.Database.Entities;
using System;
using System.Text.Json.Serialization;

namespace PFM.Backend.Models.Queries
{
    public class SpendingAnalyticsQueryParameters
    {
        [FromQuery(Name = "catcode")]
        public string? Catcode { get; set; }

        [FromQuery(Name = "start-date")]
        public DateTime? StartDate { get; set; }

        [FromQuery(Name = "end-date")]
        public DateTime? EndDate { get; set; }

        [FromQuery(Name = "direction")]
        public Direction? Direction { get; set; } 
    }
}
