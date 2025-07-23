
namespace PFM.Backend.Models.Responses
{
    public class PagedResult<T>
    {
        public int PageSize { get; set; }

        public int Page { get; set; }

        public int TotalCount { get; set; }
        
        public string SortBy { get; set; }

        public string SortOrder { get; set; }

        public List<T> Items { get; set; } = new();
    }
}
