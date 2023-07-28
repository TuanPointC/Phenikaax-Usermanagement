using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.UI.Services;

public interface IAccountService
{
    Task<bool> LoginAsync(UserLogin model);
    Task<bool> LogoutAsync();
    Task<UserDto> GetUserInformationAsync();
    public Task<bool> ChangePasswordAsync(ChangePasswordModel model);
    public Task<bool> ForceChangePasswordAsync(ForceChangePasswordModel model);
}
