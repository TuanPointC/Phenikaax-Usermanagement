using UserManagement.Shared.Models;

namespace UserManagement.Api.Services;

public interface IAuthService
{
    public Task<User?> AuthenticateAsync(UserLogin userLogin);
    public Task ChangePasswordAsync(ChangePasswordModel changePassword);
    public Task ForceChangePasswordAsync(ForceChangePasswordModel changePassword);
}
