using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Repositorys;

public interface IUsersRepo
{
    public Task<List<User>> GetAllUsersAsync();
    public Task<PagedList<UserDto>> GetUsersAsync(UserParams userParams);
    public Task<User?> GetUsersByIdAsync(Guid id);
    public Task<User?> CreateUserAsync(User? user);
    public Task<User?> UpdateUserAsync(User user);
    public Task DeleteUserAsync(Guid id);
}
