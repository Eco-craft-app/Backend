using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Modules.Users.Database;

namespace HackHeroes.Recycle.Extensions;

public static class MigrationsExtension
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var projectsDbContext = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();
        var usersDbContext = scope.ServiceProvider.GetRequiredService<UsersProfilesDbContext>();

        projectsDbContext.Database.Migrate();
        usersDbContext.Database.Migrate();
    }
}
