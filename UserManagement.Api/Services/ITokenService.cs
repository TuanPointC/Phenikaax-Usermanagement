using System.Security.Claims;
using UserManagement.Shared.Models;

namespace UserManagement.Api.Services;

public interface ITokenService
{
    public string GenerateAccessTokenAsync(User user);
    public string GenerateRefreshTokenAsync();
    public Task<bool> CheckBlackListAccessTokenAsync(string accessToken);
}
