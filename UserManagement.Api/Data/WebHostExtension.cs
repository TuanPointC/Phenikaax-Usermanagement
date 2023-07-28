namespace UserManagement.Api.Data;

public static class WebHostExtension
{
    public static WebApplication SeedData(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<UsersDbContext>();
            var config = services.GetRequiredService<IConfiguration>();
            var seeder = new DbInitializer(context, config);
            seeder.Seed();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred initializing the DB.");
        }

        return webApplication;
    }
}
