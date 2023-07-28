using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using UserManagement.Shared.Models;

namespace UserManagement.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IDatabase _redisCache;

    public TokenService(IConfiguration configuration, IDatabase redisCache)
    {
        _configuration = configuration;
        _redisCache = redisCache;
    }
    public string GenerateAccessTokenAsync(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt key is required")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new("UserName",user.UserName ?? ""),
            new("UserId",user.Id.ToString())
        };
        if (user.Roles != null)
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name ?? ""));
            }

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> CheckBlackListAccessTokenAsync(string accessToken)
    {
        var blackListAccessTokenByte = await _redisCache.ListRangeAsync("blacklist");
        foreach (var listA in blackListAccessTokenByte)
        {
            if (accessToken == listA.ToString())
            {
                return true;
            }
        }
        return false;
    }
}
