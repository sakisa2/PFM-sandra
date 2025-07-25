using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFM.Backend.Database.Entities;
using PFM.Backend.Database.Entities.CategoriesDTO;
using PFM.Backend.Models.Queries;
using PFM.Backend.Models.Requests;
using PFM.Backend.Models.Responses;
using PFM.Backend.Models.Validation;
using PFM.Backend.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PFM.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionImportService _importService;
        private readonly ITransactionService _transactionService;

        public TransactionsController(TransactionImportService importService, ITransactionService transactionService)
        {
            _importService = importService;
            _transactionService = transactionService;
        }


        [HttpPost("import")]
        [Consumes("application/csv")]
        public async Task<IActionResult> ImportTransactions()
        {
            Request.EnableBuffering();

            using var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var result = await _importService.ImportTransactionsValidatedAsync(memoryStream);

            if (!result.IsValid)
            {
                return BadRequest(result); // 400 - validacione greske
            }

            return Ok("Transactions imported successfully."); // 200 - ok
        }


        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] TransactionQueryParameters query)
        {
            var result = await _transactionService.GetTransactionsAsync(query);
            return Ok(result);
        }


        [HttpPost("{id}/categorize")] 
        public async Task<IActionResult> CategorizeTransaction(string id, [FromBody] CategorizeTransactionDTO dto)
        {
            var success = await _transactionService.CategorizeTransactionAsync(id, dto.CategoryCode);

            if (!success)
                return NotFound("Either transaction or category not found");

            return Ok("Transaction categorized successfully");
        }

        [Tags("Analytics")]
        [HttpGet("/spending-analytics")]
        [Consumes("application/csv")]
        // public async Task<IActionResult> GetSpendingAnalytics([FromQuery] SpendingAnalyticsQueryParameters query)
        // {
        //     var result = await _transactionService.GetSpendingAnalyticsAsync(
        //        query.Catcode,
        //        query.StartDate,
        //        query.EndDate,
        //        query.Direction
        //    );

        //     return Ok(result);
        // }
        public async Task<IActionResult> GetSpendingAnalytics([FromQuery] SpendingAnalyticsQueryParameters query)
        {
            var validationResult = new ValidationResult();

            if (query.StartDate.HasValue && query.EndDate.HasValue &&
                query.StartDate > query.EndDate)
            {
                validationResult.AddError("start-date", "invalid-range", "Start date must be before or equal to end date");
            }

            if (query.Direction != null &&
                !Enum.IsDefined(typeof(Direction), query.Direction))
            {
                validationResult.AddError("direction", "invalid-value", $"Direction value '{query.Direction}' is not valid");
            }

            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var result = await _transactionService.GetSpendingAnalyticsAsync(
                query.Catcode,
                query.StartDate,
                query.EndDate,
                query.Direction
            );

            return Ok(result);
        }


        [HttpPost("{id}/split")]
        public async Task<IActionResult> SplitTransaction(string id, [FromBody] List<SplitItemDTO> splits)
        {
            var success = await _transactionService.SplitTransactionAsync(id, splits);

            if (!success)
                return NotFound("Transaction or one of the categories not found.");

            return Ok("Transaction split successfully.");
        }

    }
}
