using PFM.Backend.Database.Entities.CategoriesDTO;

namespace PFM.Backend.Services
{
public interface ICategoryRepository
{
    Task<(bool success, List<string> errors)> SaveCategoriesAsync(CreateCategoryListDTO categories);
}
}