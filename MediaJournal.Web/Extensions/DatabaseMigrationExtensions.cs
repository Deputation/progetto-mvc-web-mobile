using Microsoft.EntityFrameworkCore;

public static class DatabaseMigrationExtensions
{
    public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<T>>();
        
        try
        {
            var context = services.GetRequiredService<T>();
            context.Database.Migrate();
            logger.LogInformation("Database migrated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }

        return host;
    }
}