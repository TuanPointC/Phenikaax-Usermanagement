using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Api.Data;
using UserManagement.Api.Repositorys;
using UserManagement.Shared.Models;

namespace UserManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IAuthRepo _authRepo;

    public AuthService(IConfiguration configuration, IAuthRepo authRepo)
    {
        _configuration = configuration;
        _authRepo = authRepo;
    }

    public async Task<User?> AuthenticateAsync(UserLogin userLogin)
    {
        var currentUser = await _authRepo.AuthenticateAsync(userLogin)!;
        return currentUser;
    }

    public async Task ChangePasswordAsync(ChangePasswordModel changePassword)
    {
        if (changePassword.NewPassword != changePassword.NewPasswordConfirm)
        {
            throw new Exception("The New Password and New password Confirm must match");
        }
        await _authRepo.ChangePasswordAsync(changePassword);
    }

    public async Task ForceChangePasswordAsync(ForceChangePasswordModel changePassword)
    {
        await _authRepo.ForceChangePasswordAsync(changePassword);
    }
}
