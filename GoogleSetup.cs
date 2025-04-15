using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace HelloSpaHosting;

public static class GoogleSetup
{
    public static void AddGoogleLogin(this IServiceCollection services, ConfigurationManager configuration)
    {
        // This configures Google.Apis.Auth.AspNetCore3 for use in this app.
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/api/login";
            options.LogoutPath = "/api/logout";
            options.ExpireTimeSpan = TimeSpan.FromDays(30); // Long-living session
            options.SlidingExpiration = true; // Extend on activity

            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                    //// For API calls, return 401 instead of redirecting
                    //if (context.Request.Path.StartsWithSegments("/api"))
                    //{
                    //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //    return Task.CompletedTask;
                    //}

                    //// Otherwise, do the default redirect
                    //context.Response.Redirect(context.RedirectUri);
                    //return Task.CompletedTask;
                }
            };

        })
        .AddGoogle(options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"]!;
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;

            options.ClaimActions.MapJsonKey("picture", "picture");

            options.Events = new OAuthEvents
            {
                //OnCreatingTicket = context =>
                //{

                //    // Access tokens and ID token
                //    var accessToken = context.AccessToken;
                //    var idToken = context.TokenResponse?.Response.RootElement
                //        .GetProperty("id_token").GetString();

                //    // You can log them or decode them below
                //    Console.WriteLine("ID Token: " + idToken);
                //    Console.WriteLine("Access Token: " + accessToken);
                //},
            };

            // Optional: enable silent login fallback
            //options.Events = new OAuthEvents
            //{
            //    OnRedirectToAuthorizationEndpoint = context =>
            //    {
            //        // Add prompt=none to avoid re-login popup if user has valid Google session
            //        var uri = context.RedirectUri;
            //        if (!uri.Contains("prompt="))
            //            uri += "&prompt=none";

            //        context.Response.Redirect(uri);
            //        return Task.CompletedTask;
            //    }
            //};
        });
    }
}
