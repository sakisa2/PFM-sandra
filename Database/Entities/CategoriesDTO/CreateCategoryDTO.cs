namespace PFM.Backend.Database.Entities.CategoriesDTO
{
public class CreateCategoryDTO
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? ParentCode { get; set; }
}
}