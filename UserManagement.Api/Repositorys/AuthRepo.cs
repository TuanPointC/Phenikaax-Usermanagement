using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Shared.Models;

namespace UserManagement.Api.Repositorys;

public class AuthRepo :IAuthRepo
{
    private readonly UsersDbContext _usersDbContext;

    public AuthRepo(UsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public async Task<User?>? AuthenticateAsync(UserLogin user)
    {
        var currentUsers = await _usersDbContext.Users.Where(u =>
            string.Equals(u.UserName, user.UserName)).Include(u=>u.Roles).ToListAsync();
        var res =  currentUsers.FirstOrDefault(u =>
            u.Password != null && user.Password != null && VerifyPassword(user.Password, u.Password));
        return res;
    }

    public async Task ChangePasswordAsync(ChangePasswordModel changePassword)
    {
        var currentUsers = await _usersDbContext.Users.FirstOrDefaultAsync(u => u.Id == changePassword.UserId);
        if(currentUsers == null) throw new Exception("User not found");
        if (!BCrypt.Net.BCrypt.Verify(changePassword.OldPassword, currentUsers.Password))
        {
            throw new Exception("Old Password is incorrect");
        }
        currentUsers.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);
        await _usersDbContext.SaveChangesAsync();
    }

    public async Task ForceChangePasswordAsync(ForceChangePasswordModel changePassword)
    {
        var currentUsers = await _usersDbContext.Users.FirstOrDefaultAsync(u => u.Id == changePassword.UserId);
        if(currentUsers == null) throw new Exception("User not found");
        currentUsers.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);
        await _usersDbContext.SaveChangesAsync();
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var res =  BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        return res;
    }
}
