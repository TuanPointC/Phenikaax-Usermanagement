using UserManagement.Shared.Models;

namespace UserManagement.UI.Services;

public interface ITokenManagerService
{
    public Task<string?> GetTokenAsync();
    public Task<string?> RefreshTokenEndPointAsync(TokenModel tokenModel);
}
