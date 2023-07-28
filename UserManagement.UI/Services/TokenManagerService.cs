using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using UserManagement.Shared.Models;

namespace UserManagement.UI.Services;

public class TokenManagerService : ITokenManagerService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;

    public TokenManagerService(HttpClient httpClient, ILocalStorageService localStorageService, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _navigationManager = navigationManager;
    }
    private static bool ValidateTokenExpiration(string token)
    {
        JwtSecurityToken jwt;
        try
        {
             jwt = new JwtSecurityToken(token);
        }
        catch
        {
            return false;
        }

        var date = jwt.ValidTo;
        var current = DateTime.UtcNow;
        var res = date > current;
        return res;
    }
    
    public async Task<string?> RefreshTokenEndPointAsync(TokenModel tokenModel)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Auth/RefreshToken", tokenModel);
        if (!response.IsSuccessStatusCode)
        {
                _navigationManager.NavigateTo("/login");
                await _localStorageService.RemoveItemAsync("token");
                await _localStorageService.RemoveItemAsync("refreshToken");
                return string.Empty;
        }
        var authResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        await _localStorageService.SetItemAsync("token", authResponse?.AccessToken);
        await _localStorageService.SetItemAsync("refreshToken", authResponse?.RefreshToken);
        return authResponse?.AccessToken;
    }

    public async Task<string?> GetTokenAsync()
    {
        var token = await _localStorageService.GetItemAsync<string>("token");
        var userId = await _localStorageService.GetItemAsync<string>("UserId");
        if (string.IsNullOrEmpty(token))
        {
            return string.Empty;
        }

        if (ValidateTokenExpiration(token))
        {
            return token;
        }

        var refreshToken = await _localStorageService.GetItemAsync<string>("refreshToken");
        if (string.IsNullOrEmpty(refreshToken))
        {
            return string.Empty;
        }

        var tokenModel = new TokenModel { AccessToken = token, RefreshToken = refreshToken,UserId = new Guid(userId)};
        var access =  await RefreshTokenEndPointAsync(tokenModel);
        return access;
    }
}
