using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Modules.Projects.Models.DTOs;
using Modules.Projects.Services;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.Projects;
public class GetProject
{
    public record Query(Guid ProjectId) : IRequest<Result<ProjectDto>>;

    internal sealed class Handler(ProjectsDbContext context, IUserContextService userContext) : IRequestHandler<Query, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .Include(p => p.Photos)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId, cancellationToken);


            if (project is null)
            {
                return Result.Failure<ProjectDto>(Error.NotFound("Project not found."));
            }

            var projectDto = project.Adapt<ProjectDto>();

            var userId = userContext.GetUserId();

            if(userId is not null)
            {
                 if(project.Likes.Any(l => l.UserId == userId))
                {
                    projectDto.IsLikedByCurrentUser = true;
                }

            }


            return Result.Success(projectDto);
        }
    }
}


public class GetProjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/projects/{projectId:guid}", async(Guid projectId, ISender sender) =>
        {
            var result = await sender.Send(new GetProject.Query(projectId));
            
            if(result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        })
        //.RequireAuthorization()
        .WithTags("Projects"); 
    }
}
