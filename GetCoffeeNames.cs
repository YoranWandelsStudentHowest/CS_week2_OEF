using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Howest.Functions
{
    public class GetCoffeeNames
    {
        private readonly ILogger<GetCoffeeNames> _logger;

        public GetCoffeeNames(ILogger<GetCoffeeNames> logger)
        {
            _logger = logger;
        }

        // Get all coffee names
        [Function("GetCoffeeNames")]
        public async Task<IActionResult> GetAllCoffeeNames([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "coffees")] HttpRequest req)
        {
            try
            {
                var repository = new TransactionRepository();
                var coffeeNames = await repository.GetAllCoffeeNamesAsync();
                return new OkObjectResult(coffeeNames);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting coffee names: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
