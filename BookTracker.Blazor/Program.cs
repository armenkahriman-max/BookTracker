using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookTracker.Blazor;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
?? throw new InvalidOperationException("ApiBaseUrl is missing.");

builder.Services.AddScoped<IAuthSession, AuthSession>();
builder.Services.AddTransient<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
{
    var authorizationHandler = sp.GetRequiredService<AuthorizationMessageHandler>();
    authorizationHandler.InnerHandler = new HttpClientHandler();

    return new HttpClient(authorizationHandler)
    {
        BaseAddress = new Uri(apiBaseUrl)
    };
});

builder.Services.AddScoped<BookTrackerClient>();

await builder.Build().RunAsync();
