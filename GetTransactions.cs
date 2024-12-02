using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Howest.Functions
{
    public class GetTransactions
    {
        private readonly ILogger<GetTransactions> _logger;

        public GetTransactions(ILogger<GetTransactions> logger)
        {
            _logger = logger;
        }

        [Function("GetTransactions")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions")] HttpRequest req)
        {
            _logger.LogInformation("Processing GetTransactions request.");

            try
            {
                // Create an instance of the repository
                TransactionRepository repository = new TransactionRepository();

                // Fetch transactions from the database
                List<CoffeeTransaction> transactions = await repository.GetAllTransactionsAsync();

                // Return the transactions as a JSON response
                return new OkObjectResult(transactions);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"An error occurred while fetching transactions: {ex.Message}");

                // Return a 500 Internal Server Error status
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
