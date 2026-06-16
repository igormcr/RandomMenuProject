using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RandomMenuProject.Models;

public class FoodItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}