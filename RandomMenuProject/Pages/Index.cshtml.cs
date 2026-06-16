using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RandomMenuProject.Services;
using RandomMenuProject.ViewModels;

namespace RandomMenuProject.Pages;

public class IndexModel : PageModel
{
    private readonly FoodService _foodService;

    [BindProperty]
    public string? NewFood { get; set; }

    [BindProperty]
    public int NumberOfPlates { get; set; } = 3;

    public IndexViewModel ViewModel { get; set; } = new();

    public IndexModel(FoodService foodService)
    {
        _foodService = foodService;
    }

    public async Task OnGetAsync()
    {
        ViewModel.Foods = await _foodService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAddFoodAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewFood))
        {
            try
            {
                await _foodService.AddAsync(NewFood.Trim());
            }
            catch (InvalidOperationException ex)
            {
                ViewModel.Foods = await _foodService.GetAllAsync();
                ViewModel.ErrorMessage = ex.Message;
                return Page();
            }
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteFoodAsync(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            await _foodService.DeleteAsync(id);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostGenerateMenu()
    {
        var foods = await _foodService.GetAllAsync();
        ViewModel.Foods = foods;

        // Make sure we don't try to get more plates than available foods
        var platesToGenerate = Math.Min(NumberOfPlates, foods.Count);

        var rng = new Random();
        var foodNames = foods.Select(f => f.Name).ToList();

        // Shuffle and take only the requested number
        ViewModel.Menu = foodNames.OrderBy(x => rng.Next()).Take(platesToGenerate).ToList();
        ViewModel.NumberOfPlates = NumberOfPlates;

        return Page();
    }
}