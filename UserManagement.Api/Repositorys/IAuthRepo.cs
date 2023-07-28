using UserManagement.Shared.Models;

namespace UserManagement.Api.Repositorys;

public interface IAuthRepo
{
    public Task<User?>? AuthenticateAsync(UserLogin user);
    public Task ChangePasswordAsync(ChangePasswordModel changePassword);
    public Task ForceChangePasswordAsync(ForceChangePasswordModel changePassword);
}
