using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Services;

public interface IUsersService
{
    public Task<List<UserDto>> GetAllUsersAsync();
    public Task<PagedList<UserDto>> GetUsersAsync(UserParams userParams);
    public Task<UserDto?> GetUsersByIdAsync(Guid id);
    public Task<User?> GetUserNotDtoByIdAsync(Guid id);
    public Task<UserDto?> CreateUserAsync(UserDto user, string password);
    public Task<UserDto?> UpdateUserAsync(UserDto user);
    public Task DeleteUserAsync(Guid id);
}
