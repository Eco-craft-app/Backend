using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;

namespace HackHeroes.Recycle.Extensions;

public static class MigrationsExtension
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();

        dbContext.Database.Migrate();
    }
}
