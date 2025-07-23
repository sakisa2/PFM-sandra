using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFM.Backend.Database.Entities
{
    public class Category
    {
        [Key]
        public string Code { get; set; } = string.Empty;

        [ForeignKey("Parent")]
        public string? ParentCode { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public Category? Parent { get; set; }

        public ICollection<Category> Children { get; set; } = new List<Category>();
    }
}
