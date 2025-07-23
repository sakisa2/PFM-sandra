using CsvHelper;
using CsvHelper.Configuration;
using PFM.Backend.Database.Entities.CategoriesDTO;
using System.Globalization;
using System.Text;
using PFM.Backend.Services;

namespace PFM.Backend.Services
{
    public class CategoryImportService : ICategoryImportService
    {
        public async Task<CreateCategoryListDTO> ParseCsvToDTOAsync(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream, new UTF8Encoding(false));
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            });

            csv.Context.RegisterClassMap<CSVCategoryMap>();

            var records = csv.GetRecords<CreateCategoryDTO>().ToList();

            return new CreateCategoryListDTO
            {
                Categories = records
            };
        }
    }
}
