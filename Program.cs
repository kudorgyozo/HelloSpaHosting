using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

var webRootProvider = new PhysicalFileProvider(builder.Environment.WebRootPath);
var clientAppProvider = new PhysicalFileProvider(
  Path.Combine(builder.Environment.ContentRootPath, "ClientApp\\dist"));

var compositeProvider = new CompositeFileProvider(webRootProvider,
                                                  clientAppProvider);

// Update the default provider.
app.Environment.WebRootFileProvider = compositeProvider;

app.UseStaticFiles();
//app.MapStaticAssets();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapFallbackToFile("/index.html");



app.Run();


internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
