using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;
namespace UserManagement.UI.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenManagerService _tokenManagerService;
    private readonly ILocalStorageService _localStorageService;


    public UserService(HttpClient httpClient, ILocalStorageService localStorageService, ITokenManagerService tokenManagerService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _tokenManagerService = tokenManagerService;
    }

    public async Task<PagedResult<UserDto>?> GetUsersAsync(UserParams userParams)
    {
        var token =  await _tokenManagerService.GetTokenAsync(); 
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        var builder = new UriBuilder($"{_httpClient.BaseAddress}api/Users/GetUsers");
        var query = HttpUtility.ParseQueryString(builder.Query);
        if (!string.IsNullOrEmpty(userParams.SearchName))
        {
            query["SearchName"] = userParams.SearchName;
        }
        query["PageNumber"] = userParams.PageNumber.ToString();
        query["PageSize"] = userParams.PageSize.ToString();
        builder.Query = query.ToString();
        var url = builder.ToString();
        try
        {
            var response =  await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PagedResult<UserDto>>();
            }
            return null;
        }
        catch(Exception e)
        {
            var error = e.Message;
            return null;
        }
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var token =  await _tokenManagerService.GetTokenAsync(); 
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        var builder = new UriBuilder($"{_httpClient.BaseAddress}api/Users/GetUserById");
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["userId"] = id;
        builder.Query = query.ToString();
        var url = builder.ToString();
        try{
            return await _httpClient.GetFromJsonAsync<UserDto>(url) ?? new UserDto();
        }
        catch
        {
            return new UserDto();
        }
    }

    public async Task<bool> UpdateUserAsync(UserDto userDto)
    {
        var token =  await _tokenManagerService.GetTokenAsync(); 
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        var builder = new UriBuilder($"{_httpClient.BaseAddress}api/Users/UpdateUser");
        var url = builder.ToString();
        var json = JsonConvert.SerializeObject(userDto);
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

    public async Task<bool> DeleteUserAsync(string id)
    {
        var token =  await _tokenManagerService.GetTokenAsync(); 
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        var builder = new UriBuilder($"{_httpClient.BaseAddress}api/Users/DeleteUser");
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["userId"] = id;
        builder.Query = query.ToString();
        var url = builder.ToString();
        try{
            var res = await _httpClient.DeleteAsync(url);
            return res.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddUserAsync(UserDto userDto, string? password)
    {
        var token =  await _tokenManagerService.GetTokenAsync(); 
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        var builder = new UriBuilder($"{_httpClient.BaseAddress}api/Users/CreateUser");
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["password"] = password;
        builder.Query = query.ToString();
        var url = builder.ToString();
        var json = JsonConvert.SerializeObject(userDto);
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
