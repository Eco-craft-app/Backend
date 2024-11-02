using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Projects.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Extensions;
internal static class ServiceCollectionExtensions
{
    internal static void AddProjectsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProjectsDatabase(configuration);
    }

    private static void AddProjectsDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProjectsDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("RecycleDb"));
        });
    }

}
