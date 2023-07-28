using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.UI.Services;

public interface IUserService
{
    public Task<PagedResult<UserDto>?> GetUsersAsync(UserParams userParams);
    public Task<UserDto> GetUserByIdAsync(string id);
    public Task<bool> UpdateUserAsync(UserDto userDto);
    public Task<bool> DeleteUserAsync(string id);
    public Task<bool> AddUserAsync(UserDto userDto,string? password);
}
