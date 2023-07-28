using UserManagement.Shared.Models;

namespace UserManagement.Api.Data;

public class RolesSeeder
{
    private readonly UsersDbContext _userDbContext;
    private readonly IConfiguration _configuration;

    public RolesSeeder(UsersDbContext userDbContext, IConfiguration configuration)
    {
        _userDbContext = userDbContext;
        _configuration = configuration;
    }
    public void Seed()
    {
        var roles = _configuration.GetSection("SeedRoles").Get<List<Role>>();
        if (roles == null) return;
        _userDbContext.Roles.AddRange(roles);
        _userDbContext.SaveChanges();
    }
}
