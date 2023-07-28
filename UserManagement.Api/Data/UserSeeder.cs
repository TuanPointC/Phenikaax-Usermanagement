using UserManagement.Shared.Models;

namespace UserManagement.Api.Data;

public class UserSeeder
{
    private readonly UsersDbContext _userDbContext;
    private readonly IConfiguration _configuration;

    public UserSeeder(UsersDbContext userDbContext, IConfiguration configuration)
    {
        _userDbContext = userDbContext;
        _configuration = configuration;
    }

    public void Seed()
    {
        var roleNames = _configuration.GetValue<string>("SeedUsers:Role")?.Split(",").ToList() ?? new List<string>{"User"};
        var roles = _userDbContext.Roles.Where(r => r.Name != null && roleNames.Contains(r.Name)).ToList();
        var user = new User
        {
            UserName = _configuration.GetValue<string>("SeedUsers:UserName")??"",
            Password = BCrypt.Net.BCrypt.HashPassword(_configuration.GetValue<string>("SeedUsers:Password")),
            Roles = roles
        };
        _userDbContext.Users.Add(user);
        _userDbContext.SaveChanges();
    }
}
