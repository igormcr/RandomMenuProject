using RandomMenuProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Get port from environment variable (Railway sets this)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://+:{port}");

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddSingleton<FoodService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();