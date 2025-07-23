using Microsoft.AspNetCore.Http;

namespace PFM.Backend.Models.Requests
{
    public class CategoryImportRequest
    {
        public IFormFile File { get; set; }
    }
}
