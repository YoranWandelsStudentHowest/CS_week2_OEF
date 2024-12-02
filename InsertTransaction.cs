using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Howest.Functions
{
    public class InsertTransaction
    {
        private readonly ILogger<InsertTransaction> _logger;

        public InsertTransaction(ILogger<InsertTransaction> logger)
        {
            _logger = logger;
        }

        // Insert a new transaction
        [Function("InsertTransaction")]
        public async Task<IActionResult> InsertNewTransaction([HttpTrigger(AuthorizationLevel.Function, "post", Route = "transactions")] HttpRequest req)
        {
            try
            {
                var repository = new TransactionRepository();
                var transaction = await req.ReadFromJsonAsync<CoffeeTransaction>();

                if (transaction == null)
                {
                    return new BadRequestObjectResult("Invalid transaction data.");
                }

                await repository.InsertTransactionAsync(transaction);

                return new CreatedResult("/api/transactions", transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inserting transaction: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
