using PFM.Backend.Database.Entities.CategoriesDTO;
using PFM.Backend.Models.Validation;

namespace PFM.Backend.Services
{
public interface ICategoryRepository
{
        Task<(bool success, List<ValidationError> errors)> SaveCategoriesAsync(CreateCategoryListDTO categories);

}
}