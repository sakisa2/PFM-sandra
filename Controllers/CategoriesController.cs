using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFM.Backend.Models.Requests;
using PFM.Backend.Services;
using PFM.Backend.Database.Entities.CategoriesDTO;

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
        [Consumes("application/csv")]
        public async Task<IActionResult> ImportCategories([FromForm] CategoryImportRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { error = "File is required." });
            }

            CreateCategoryListDTO parsed;
            try
            {
                using var stream = request.File.OpenReadStream();
                parsed = await _categoryImportService.ParseCsvToDTOAsync(stream);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Invalid CSV format", detail = ex.Message });
            }

            var (success, errors) = await _categoryRepository.SaveCategoriesAsync(parsed);

            if (!success)
            {
                return StatusCode(440, new { errors });
            }

            return Ok(new { message = "Categories imported successfully." });
        }
    }
}
