using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Howest.Functions
{
    public class ReturnTotalTransactions
    {
        private readonly ILogger<ReturnTotalTransactions> _logger;

        public ReturnTotalTransactions(ILogger<ReturnTotalTransactions> logger)
        {
            _logger = logger;
        }

        // Get the total number of transactions
        [Function("GetTotalTransactions")]
        public async Task<IActionResult> GetTotalTransactions([HttpTrigger(AuthorizationLevel.Function, "get", Route = "transactions/count")] HttpRequest req)
        {
            try
            {
                var repository = new TransactionRepository();
                var totalTransactions = await repository.GetTotalTransactionsAsync();
                return new OkObjectResult(totalTransactions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting total transactions: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
