using Microsoft.AspNetCore.Mvc;
using PFM.Backend.Services;
using Microsoft.AspNetCore.Http;

namespace PFM.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionImportService _importService;

        public TransactionsController(TransactionImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportTransactions()
        {
                Console.WriteLine(">>> ImportTransactions called <<<");

            try
            {
                Request.EnableBuffering(); 

                using var memoryStream = new MemoryStream();
                await Request.Body.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                await _importService.ImportTransactionsAsync(memoryStream);
                return Ok("Transactions imported successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
