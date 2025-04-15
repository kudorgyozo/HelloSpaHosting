//using Google.Apis.Auth.AspNetCore3;
using HelloSpaHosting;
using HelloSpaHosting.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGoogleLogin(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.

var webRootProvider = new PhysicalFileProvider(builder.Environment.WebRootPath);
var clientAppProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "ClientApp\\dist"));

var compositeProvider = new CompositeFileProvider(webRootProvider, clientAppProvider);

// Update the default provider.
app.Environment.WebRootFileProvider = compositeProvider;

app.UseStaticFiles();

app.UseHttpsRedirection();


app.MapGet("/api/login", async (HttpContext context) =>
{
    if (!context.User.Identity.IsAuthenticated)
    {
        await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = "http://localhost:4200" // Or your Angular app route
        });
    } else
    {
        context.Response.Redirect("http://localhost:4200");
    }
});

app.MapGet("/api/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("http://localhost:4200");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

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

app.MapGet("/api/user", [Authorize] (IHttpContextAccessor ctx) =>
{
    return new
    {
        ctx.HttpContext!.User.Identity?.Name,
        Email = ctx.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value,
        Picture = ctx.HttpContext.User.FindFirst("picture")?.Value
    };

    //var user = ctx.HttpContext!.User;

    //var name = ctx.HttpContext.User.FindFirst("name")?.Value;
    //var email = ctx.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
    //var pictureUrl = ctx.HttpContext.User.FindFirst("picture")?.Value;

    //return new UserInfo(name, email, pictureUrl);
});

//app.MapGet("/google-response", async (IHttpContextAccessor ctx) =>
//{
//    var result = await ctx.HttpContext.AuthenticateAsync();
//    if (result?.Properties?.Items?.TryGetValue("sessionId", out var loginRequestId) == true)
//    {
//        Console.WriteLine($"SessionId: {loginRequestId}");
//        // do something with that data
//    }

//    return Results.Redirect("http://localhost:4200");
//});



app.MapGet("/api/userinfo", [Authorize] (ClaimsPrincipal user) =>
{
    return new
    {
        Name = user.Identity?.Name,
        Email = user.FindFirst(ClaimTypes.Email)?.Value
    };
});

app.MapFallbackToFile("/index.html");

app.Run();
