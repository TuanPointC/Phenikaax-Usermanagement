using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;
using UserManagement.UI.Auth;

namespace UserManagement.UI.Services;

public class AccountService : IAccountService
{
    private readonly AuthenticationStateProvider _customAuthenticationProvider;
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly ITokenManagerService _tokenManagerService;

    public AccountService(AuthenticationStateProvider customAuthenticationProvider, ILocalStorageService localStorageService, HttpClient httpClient, NavigationManager navigationManager, ITokenManagerService tokenManagerService)
    {
        _customAuthenticationProvider = customAuthenticationProvider;
        _localStorageService = localStorageService;
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _tokenManagerService = tokenManagerService;
    }

    public async Task<bool> LoginAsync(UserLogin model)
    {
        var response = await _httpClient.PostAsJsonAsync<UserLogin>("/api/Auth/Login", model);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        var authData = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        await _localStorageService.SetItemAsync("token", authData?.AccessToken);
        await _localStorageService.SetItemAsync("refreshToken", authData?.RefreshToken);
        await _localStorageService.SetItemAsync("UserId", authData?.UserId.ToString());
        
        (_customAuthenticationProvider as CustomAuthenticationProvider)?.Notify();
        return true;
    }

    public async Task<bool> LogoutAsync()
    {
        await _localStorageService.RemoveItemAsync("token");
        await _localStorageService.RemoveItemAsync("refreshToken");
        await _localStorageService.RemoveItemAsync("UserId");
        await Task.Delay(300);
        (_customAuthenticationProvider as CustomAuthenticationProvider)?.Notify();
        return true;
    }
    public async Task<UserDto> GetUserInformationAsync()
    {
        var authenticationState = await _customAuthenticationProvider.GetAuthenticationStateAsync();
        var claims = authenticationState.User.Claims;
        var user = new UserDto();
        foreach (var claim in claims)
        {
            switch (claim.Type)
            {
                case "UserName":
                    user.UserName = claim.Value;
                    break;
                case "UserId":
                    user.Id = Guid.Parse(claim.Value);
                    break;
                case ClaimTypes.Role:
                    if (string.IsNullOrEmpty(user.Roles))
                    {
                        user.Roles = claim.Value;
                    }
                    else
                    {
                        user.Roles += "," + claim.Value;
                    }
                    break;
            }
        }
        return user;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordModel model)
    {
        var token =  await _tokenManagerService.GetTokenAsync(); 
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        var builder = new UriBuilder($"{_httpClient.BaseAddress}api/Auth/ChangePassword");
        var url = builder.ToString();
        var json = JsonConvert.SerializeObject(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        try{
            var res = await _httpClient.PostAsync(url, data);
            return res.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ForceChangePasswordAsync(ForceChangePasswordModel model)
    {
        var token =  await _tokenManagerService.GetTokenAsync(); 
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        var builder = new UriBuilder($"{_httpClient.BaseAddress}api/Auth/ForceChangePassword");
        var url = builder.ToString();
        var json = JsonConvert.SerializeObject(model);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        try{
            var res = await _httpClient.PostAsync(url, data);
            return res.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }
}
