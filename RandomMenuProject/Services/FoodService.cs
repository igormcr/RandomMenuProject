using MongoDB.Driver;
using RandomMenuProject.Models;

namespace RandomMenuProject.Services;

public class FoodService
{
    private readonly IMongoCollection<FoodItem> _foods;

    public FoodService(IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("MongoDB:ConnectionString");
        var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName");
        var collectionName = configuration.GetValue<string>("MongoDB:CollectionName");

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _foods = database.GetCollection<FoodItem>(collectionName);
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