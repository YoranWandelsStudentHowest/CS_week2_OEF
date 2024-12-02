using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Howest.Functions
{
    public class CalculatorTrigger
    {
        private readonly ILogger<CalculatorTrigger> _logger;

        public CalculatorTrigger(ILogger<CalculatorTrigger> logger)
        {
            _logger = logger;
        }

        [Function("CalculatorTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "calculator")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP POST trigger function processed a request.");

            try
            {
                // Deserialize the JSON body
                var input = await req.ReadFromJsonAsync<CalculationRequest>();

                if (input == null || string.IsNullOrEmpty(input.Operator))
                {
                    return new BadRequestObjectResult("Invalid input. Please provide 'a', 'b', and 'operation'.");
                }

                int result;
                switch (input.Operator.ToLower())
                {
                    case "add":
                        result = input.A + input.B;
                        break;
                    case "subtract":
                        result = input.A - input.B;
                        break;
                    case "multiply":
                        result = input.A * input.B;
                        break;
                    case "divide":
                        if (input.B == 0)
                        {
                            return new BadRequestObjectResult("Division by zero is not allowed.");
                        }
                        result = input.A / input.B;
                        break;
                    default:
                        return new BadRequestObjectResult($"Invalid operation '{input.Operator}'. Supported operations are: add, subtract, multiply, divide.");
                }

                // Create the response
                var response = new CalculationResponse
                {
                    Result = result,
                    Operator = input.Operator
                };

                return new OkObjectResult(response);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing the request body.");
                return new BadRequestObjectResult("Invalid JSON format.");
            }
        }
    }
}
