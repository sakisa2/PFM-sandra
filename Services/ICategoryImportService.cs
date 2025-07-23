using PFM.Backend.Database.Entities.CategoriesDTO;

namespace PFM.Backend.Services
{

public interface ICategoryImportService
{
    Task<CreateCategoryListDTO> ParseCsvToDTOAsync(Stream csvStream);
}
}