namespace Howest.Repositories;

public class TransactionRepository
{
    private readonly string _connectionString;

    public TransactionRepository()
    {
        // Fetch connection string from environment variables
        _connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

        // Ensure the connection string is not null or empty
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("The connection string is not set in the environment variables.");
        }
    }

    public async Task<List<CoffeeTransaction>> GetAllTransactionsAsync()
    {
        List<CoffeeTransaction> transactions = new List<CoffeeTransaction>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT ID, DateTime, CashType, Card, Money, CoffeeName FROM Transactions";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            CoffeeTransaction transaction = new CoffeeTransaction
                            {
                                ID = reader.GetGuid(reader.GetOrdinal("ID")),
                                DateTime = reader.GetDateTime(reader.GetOrdinal("DateTime")),
                                CashType = reader["CashType"] as string,
                                Card = reader["Card"] as string,
                                Money = reader.GetDecimal(reader.GetOrdinal("Money")),
                                CoffeeName = reader["CoffeeName"] as string
                            };

                            transactions.Add(transaction);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary, then re-throw
                    // _logger?.LogError(ex, "Error fetching transactions");
                    throw; // Ensures the Azure Function runtime handles the exception
                }
            }
        }

        return transactions;
    }
    // Get all coffee names
    public async Task<List<string>> GetAllCoffeeNamesAsync()
    {
        List<string> coffeeNames = new List<string>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT DISTINCT CoffeeName FROM Transactions";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            coffeeNames.Add(reader["CoffeeName"] as string);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details (you can use any logging framework like Serilog, NLog, or log to console)
                    // For now, we are just throwing the exception to capture the error with a detailed message
                    throw new Exception($"Error fetching coffee names: {ex.Message}, StackTrace: {ex.StackTrace}");
                }
            }
        }

        return coffeeNames;
    }

    // Insert a new transaction
    public async Task InsertTransactionAsync(CoffeeTransaction transaction)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO Transactions (ID, DateTime, CashType, Card, Money, CoffeeName) " +
                           "VALUES (@ID, @DateTime, @CashType, @Card, @Money, @CoffeeName)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", transaction.ID);
                command.Parameters.AddWithValue("@DateTime", transaction.DateTime);
                command.Parameters.AddWithValue("@CashType", transaction.CashType);
                command.Parameters.AddWithValue("@Card", transaction.Card);
                command.Parameters.AddWithValue("@Money", transaction.Money);
                command.Parameters.AddWithValue("@CoffeeName", transaction.CoffeeName);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                    throw new Exception("Error inserting transaction");
                }
            }
        }
    }

    // Get the total number of transactions
    public async Task<int> GetTotalTransactionsAsync()
    {
        int totalCount = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(*) FROM Transactions";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();
                    totalCount = (int)await command.ExecuteScalarAsync();
                }
                catch (Exception)
                {
                    throw new Exception("Error fetching total transaction count");
                }
            }
        }

        return totalCount;
    }
}
