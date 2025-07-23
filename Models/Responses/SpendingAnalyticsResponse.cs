using System.Collections.Generic;

namespace PFM.Backend.Models.Responses
{
    public class SpendingAnalyticsResponse
    {
        public List<SpendingGroupDTO> Groups { get; set; } = new();
    }
}
