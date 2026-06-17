using MongoDB.Driver;
using RandomMenuProject.Models;

namespace RandomMenuProject.Services;

public class FoodService
{
    private readonly IMongoCollection<FoodItem> _foods;

    public FoodService(IConfiguration configuration)
    {
        // Use the external URL from Railway
        var connectionString = "mongodb://mongo:TuyxerCHTmgjnIjVMxCTaqIbcekxvWTC@thomas.proxy.rlwy.net:39608";
        var databaseName = "RandomMenuDB";
        var collectionName = "Foods";

        Console.WriteLine($"🔗 Connecting to MongoDB at: thomas.proxy.rlwy.net:39608");
        Console.WriteLine($"📦 Database: {databaseName}");
        Console.WriteLine($"📁 Collection: {collectionName}");

        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _foods = database.GetCollection<FoodItem>(collectionName);

            // Test the connection
            var collections = database.ListCollectionNames().ToList();
            Console.WriteLine($"✅ MongoDB connected successfully! Found {collections.Count} collections.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
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