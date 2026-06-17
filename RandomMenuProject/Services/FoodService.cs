using MongoDB.Driver;
using RandomMenuProject.Models;

namespace RandomMenuProject.Services;

public class FoodService
{
    private readonly IMongoCollection<FoodItem> _foods;

    public FoodService(IConfiguration configuration)
    {
        // Get connection string from environment variable
        var connectionString = Environment.GetEnvironmentVariable("MONGO_URL");

        // If not found, try appsettings.json
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetValue<string>("MongoDB:ConnectionString");
        }

        // Fallback to localhost for development
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "mongodb://localhost:27017";
        }

        // Get database name from environment or config
        var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME")
            ?? configuration.GetValue<string>("MongoDB:DatabaseName")
            ?? "RandomMenuDB";

        var collectionName = configuration.GetValue<string>("MongoDB:CollectionName")
            ?? "Foods";

        // Log the connection (without password for security)
        var safeConnectionString = connectionString.Contains("@")
            ? connectionString.Split('@')[1]
            : connectionString;
        Console.WriteLine($"Connecting to MongoDB at: {safeConnectionString}");
        Console.WriteLine($"Database: {databaseName}");
        Console.WriteLine($"Collection: {collectionName}");

        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _foods = database.GetCollection<FoodItem>(collectionName);

            // Test the connection
            var test = database.ListCollectionNames().ToList();
            Console.WriteLine("MongoDB connection successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MongoDB connection failed: {ex.Message}");
            throw;
        }
    }

    public async Task<List<FoodItem>> GetAllAsync()
    {
        return await _foods.Find(_ => true)
                          .SortByDescending(f => f.CreatedAt)
                          .ToListAsync();
    }

    public async Task AddAsync(string name)
    {
        var existing = await _foods.Find(f => f.Name == name).FirstOrDefaultAsync();
        if (existing != null)
        {
            throw new InvalidOperationException($"Food '{name}' already exists!");
        }

        var food = new FoodItem
        {
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        await _foods.InsertOneAsync(food);
    }

    public async Task DeleteAsync(string id)
    {
        await _foods.DeleteOneAsync(f => f.Id == id);
    }
}