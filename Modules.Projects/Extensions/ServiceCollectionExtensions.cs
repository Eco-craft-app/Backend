using Carter;
using FluentValidation;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Projects.Database;
using Modules.Projects.Features.Projects;
using Modules.Projects.Services;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Extensions;
internal static class ServiceCollectionExtensions
{
    internal static void AddProjectsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProjectsDatabase(configuration);
        services.AddServices(configuration);
        services.AddAuth(configuration);
    }


    private static void AddProjectsDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProjectsDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("RecycleDb"));
        });
    }

    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextService, UserContextService>();
        var assembly = typeof(AddProject).Assembly;
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<ISieveProcessor, ProjectsSieveProcessor>();
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.NotificationPublisher = new ForeachAwaitPublisher();
        });
        services.AddCarter();

    }



    private static void AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("admin"));
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.Audience = configuration["Authentication:Audience"];
                o.MetadataAddress = configuration["Authentication:MetadataAddress"]!;
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = configuration["Authentication:ValidIssuer"]
                };
            });



    }

}
