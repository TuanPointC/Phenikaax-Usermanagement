using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using UserManagement.Api.Services;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<User> _logger;
    private readonly IHelperService _helperService;
    private readonly IDatabase _redisCache;

    public AuthController(IUsersService usersService, ILogger<User> logger, IAuthService authService, IHelperService helperService, ITokenService tokenService, IDatabase redisCache)
    {
        _usersService = usersService;
        _logger = logger;
        _authService = authService;
        _helperService = helperService;
        _tokenService = tokenService;
        _redisCache = redisCache;
    }

    [AllowAnonymous]
    [HttpPost, Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        try
        {
            var user = await _authService.AuthenticateAsync(userLogin);
            if (user != null)
            {
                var accessToken = _tokenService.GenerateAccessTokenAsync(user);
                var refreshToken = _tokenService.GenerateRefreshTokenAsync();
                var redisTask = _redisCache.StringSetAsync(user.Id.ToString(), refreshToken, TimeSpan.FromDays(7));
                
                var result = new LoginResponse
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = string.Join(",", user.Roles?.Select(r => r.Name) ?? Array.Empty<string?>()),
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                _logger.LogInformation("Login successfully");
                await redisTask;
                return Ok(result);
            }

            return NotFound("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside Authenticate action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }

    [Authorize]
    [HttpPost]
    [Route("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        try
        {
            await _authService.ChangePasswordAsync(model);
            return Ok("Change Password successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside Authenticate action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }
    
    [Authorize(Roles = "Administrator, Distributor")]
    [HttpPost, Route("ForceChangePassword")]
    public async Task<IActionResult> ForceChangePassword(ForceChangePasswordModel model)
    {
        try
        {
            // Check right when creating user
            var identity = HttpContext.User.Identities.First();
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c=>c.Value).ToList();
            var user = await _usersService.GetUsersByIdAsync(model.UserId) ?? new UserDto{Roles = "User"};
            if (!_helperService.CompareRights(roles, user.Roles?.Split(",").ToList()))
            {
                _logger.LogError($"You do not have the right change password!"); 
                return StatusCode(500, "You do not have the right change password!");
            }
            await _authService.ForceChangePasswordAsync(model);
            return Ok("Change Password successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside Authenticate action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }

    [AllowAnonymous]
    [HttpPost, Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel? tokenModel)
    {
        try
        {
            if (tokenModel?.AccessToken != null)
            {
                var isBlackList = await _tokenService.CheckBlackListAccessTokenAsync(tokenModel.AccessToken);
                if (isBlackList)
                {
                    return BadRequest("AccessToken is old");
                }
                var accessToken = tokenModel.AccessToken;
                var refreshToken = tokenModel.RefreshToken;
                var userId = tokenModel.UserId;

                if (accessToken == null)
                    return BadRequest("AccessToken is required");

                var user = await _usersService.GetUserNotDtoByIdAsync(userId);
                var refreshTokenCached = await _redisCache.StringGetAsync(userId.ToString());
                if (user is null || refreshTokenCached != refreshToken)
                    return BadRequest("Invalid client request or refresh token provided is not valid");
                
                var newAccessToken = _tokenService.GenerateAccessTokenAsync(user);
                var newRefreshToken = _tokenService.GenerateRefreshTokenAsync();
                var listTasksRedis = new List<Task>
                {
                    _redisCache.StringSetAsync(userId.ToString(), newRefreshToken, TimeSpan.FromDays(7)),
                    _redisCache.ListLeftPushAsync("blacklist", accessToken),
                    _redisCache.KeyExpireAsync("blacklist", TimeSpan.FromDays(2))
                };

                var result = new LoginResponse()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = string.Join(",", user.Roles?.Select(r => r.Name) ?? Array.Empty<string?>()),
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
                _logger.LogInformation("Refresh token successfully");
                await Task.WhenAll(listTasksRedis);
                return Ok(result);
            }
            return StatusCode(500, "Internal server error"); 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside refresh token action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }

    [Authorize]
    [HttpPost, Route("Logout")]
    public async Task<IActionResult> Logout(Guid userId)
    {
        try
        {
            // Remove Refresh Token
            _redisCache.KeyDelete(userId.ToString());
            // Add Access Token to BlackList
            var token = await HttpContext.GetTokenAsync("access_token");
            var listTasksRedis = new List<Task>
            {
                _redisCache.ListLeftPushAsync("blacklist", token),
                _redisCache.KeyExpireAsync("blacklist", TimeSpan.FromDays(2))
            };
            await Task.WhenAll(listTasksRedis);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside logout token action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }
}
