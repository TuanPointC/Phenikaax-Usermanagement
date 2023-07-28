using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Services;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly ILogger<User> _logger;
    private readonly IHelperService _helperService;

    public UsersController(IUsersService usersService, ILogger<User> logger, IHelperService helperService)
    {
        _usersService = usersService;
        _logger = logger;
        _helperService = helperService;
    }
    
    [Authorize(Roles = "Administrator, Distributor")]
    [HttpGet]
    [Route("AllUsers")]
    public async Task<IActionResult> GetAllUser()
    {
        try
        {
            var users =  await _usersService.GetAllUsersAsync();
            _logger.LogInformation("Returned all users from databases");
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }
    
    [Authorize(Roles = "Administrator, Distributor")]
    [HttpGet]
    [Route("GetUsers")]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    {
        try
        {
            var users =  await _usersService.GetUsersAsync(userParams);
            _logger.LogInformation("Returned all users from databases");
            return Ok(new PagedResult<UserDto>(users));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside GetAllUsers action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }
    
    [Authorize(Roles = "Administrator, Distributor")]
    [HttpGet]
    [Route("GetUserById")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        try
        {
            var users =  await _usersService.GetUsersByIdAsync(userId);
            _logger.LogInformation("Returned all users from databases");
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside GetUserById action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }
    
    

    [Authorize(Roles = "Administrator, Distributor")]
    [HttpPost, Route("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user, string password)
    {
        try
        {
            // Check right when creating user
            var identity = HttpContext.User.Identities.First();
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c=>c.Value).ToList();
            if (!_helperService.CompareRights(roles, user.Roles?.Split(",").ToList()))
            {
                _logger.LogError($"You do not have the right creating an user!"); 
                return StatusCode(500, "You do not have the right creating an user!");
            }
            var createdUser = await _usersService.CreateUserAsync(user, password);
            _logger.LogInformation("Created new user");
            return Ok(createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside Create user action: {ex.InnerException?.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }

    [Authorize(Roles = "Administrator, Distributor")]
    [HttpPost, Route("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto user)
    {
        try
        {
            // Check right when updating user
            var identity = HttpContext.User.Identities.First();
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c=>c.Value).ToList();
            if (!_helperService.CompareRights(roles, user.Roles?.Split(",").ToList()))
            {
                _logger.LogError($"You do not have the right updating an user!"); 
                return StatusCode(500, "You do not have the right updating an user!");
            }
            await _usersService.UpdateUserAsync(user);
            return Ok("User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside Update user action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }
    
    [Authorize(Roles = "Administrator, Distributor")]
    [HttpDelete, Route("DeleteUser")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        try
        {
            // Check right when updating user
            var identity = HttpContext.User.Identities.First();
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c=>c.Value).ToList();
            var user = await _usersService.GetUsersByIdAsync(userId) ?? new UserDto { Id = userId, Roles = "User"};
            if (!_helperService.CompareRights(roles, user.Roles?.Split(",").ToList()))
            {
                _logger.LogError($"You do not have the right deleting an user!"); 
                return StatusCode(500, "You do not have the right deleting an user!");
            }
            await _usersService.DeleteUserAsync(userId);
            return Ok("User Delete successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside Delete user action: {ex.Message}"); 
            return StatusCode(500, "Internal server error"); 
        }
    }
}
