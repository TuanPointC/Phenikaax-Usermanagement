namespace UserManagement.Api.Data;

public class DbInitializer
{
    private readonly UsersDbContext _db;
    private readonly IConfiguration _configuration;
    public DbInitializer(UsersDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    public void Seed()
    {
        _db.Database.EnsureCreated();
        // Seed Roles
        if (!_db.Roles.Any())
        {
            var roleSeeder = new RolesSeeder(_db,_configuration);
            roleSeeder.Seed();
        }
        // Seed Users
        if (!_db.Users.Any())
        {
            var seeder = new UserSeeder(_db,_configuration);
            seeder.Seed();   
        }
    }
}
