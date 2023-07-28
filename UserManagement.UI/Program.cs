using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using UserManagement.UI;
using UserManagement.UI.Auth;
using UserManagement.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7205/")
});
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITokenManagerService, TokenManagerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.VisibleStateDuration = 1000;
        config.SnackbarConfiguration.HideTransitionDuration = 100;
        config.SnackbarConfiguration.ShowTransitionDuration = 100;
        config.SnackbarConfiguration.PreventDuplicates = true;
    }
);

await builder.Build().RunAsync();
