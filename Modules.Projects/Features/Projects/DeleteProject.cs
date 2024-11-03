using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Modules.Projects.Services;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.Projects;
public class DeleteProject
{
    public record DeleteProjectCommand(Guid ProjectId) : IRequest<Result>;

    internal sealed class DeleteProjectHandler(ProjectsDbContext context, IUserContextService userContext) : IRequestHandler<DeleteProjectCommand, Result>
    {
        public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = userContext.GetUserId();

            var project = await context.Projects.FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId);

            if (project is null)
            {
                return Result.Failure(Error.NotFound("Project not found."));
            }

            if (project.UserId != userId)
            {
                return Result.Failure(Error.Forbidden());
            }

            context.Projects.Remove(project);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}


public class DeleteProjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/projects/{projectId:guid}", async (Guid projectId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteProject.DeleteProjectCommand(projectId));

            if (result.IsFailure)
            {
                if(result.Error == Error.Forbidden())
                {
                    return Results.Forbid();
                }

                return Results.NotFound(result.Error);
            }

            return Results.NoContent();
        })
        //.RequireAuthorization()
        .WithTags("Projects");
    }
}