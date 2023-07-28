using Microsoft.EntityFrameworkCore;
using UserManagement.Shared.Models;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace UserManagement.Api.Data;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
    }
}
