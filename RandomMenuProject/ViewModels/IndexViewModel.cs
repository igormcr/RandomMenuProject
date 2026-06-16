using RandomMenuProject.Models;

namespace RandomMenuProject.ViewModels;

public class IndexViewModel
{
    public List<FoodItem> Foods { get; set; } = new();
    public List<string>? Menu { get; set; }
    public string? ErrorMessage { get; set; }
    public int NumberOfPlates { get; set; } = 3; // Default to 3 plates
}