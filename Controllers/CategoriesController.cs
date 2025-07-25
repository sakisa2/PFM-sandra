using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFM.Backend.Database.Entities.CategoriesDTO;
using PFM.Backend.Models.Requests;
using PFM.Backend.Models.Validation;
using PFM.Backend.Services;

namespace PFM.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryImportService _categoryImportService;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(
            ICategoryImportService categoryImportService,
            ICategoryRepository categoryRepository)
        {
            _categoryImportService = categoryImportService;
            _categoryRepository = categoryRepository;
        }


        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportCategories([FromForm] CategoryImportRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new
                {
                    errors = new List<ValidationError>
            {
                new ValidationError("file", "required", "CSV file is required.")
            }
                });
            }

            CreateCategoryListDTO parsed;
            var preValidationErrors = new List<ValidationError>();

            try
            {
                using var stream = request.File.OpenReadStream();
                parsed = await _categoryImportService.ParseCsvToDTOAsync(stream);
            }
            catch
            {
                return BadRequest(new
                {
                    errors = new List<ValidationError>
            {
                new ValidationError("file", "invalid-format", "Invalid CSV format.")
            }
                });
            }

            foreach (var cat in parsed.Categories)
            {
                if (string.IsNullOrWhiteSpace(cat.Code))
                    preValidationErrors.Add(new ValidationError("code", "missing-field", "Code is required."));

                if (string.IsNullOrWhiteSpace(cat.Name))
                    preValidationErrors.Add(new ValidationError("name", "missing-field", "Name is required."));
            }

            if (preValidationErrors.Any())
            {
                return BadRequest(new { errors = preValidationErrors });
            }

            var (success, businessErrors) = await _categoryRepository.SaveCategoriesAsync(parsed);

            if (!success)
            {
                return StatusCode(440, new
                {
                    problem = "category-import-failed",
                    message = "Some categories could not be imported due to business rule violations.",
                    details = businessErrors.Select(e => e.Message).ToList()
                });
            }

            return Ok(new { message = "Categories imported successfully." });
        }

    }
}
