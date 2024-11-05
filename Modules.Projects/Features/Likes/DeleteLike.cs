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

namespace Modules.Projects.Features.Likes;
public class DeleteLike
{
    public record DeleteLikeCommand(Guid ProjectId) : IRequest<Result>;

    internal sealed class Handler(ProjectsDbContext context, IUserContextService userContext) : IRequestHandler<DeleteLikeCommand, Result>
    {
        public async Task<Result> Handle(DeleteLikeCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .FirstOrDefaultAsync(x => x.ProjectId == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return Result.Failure(Error.NotFound("Project not found"));
            }

            var userId = userContext.GetUserId();

            var like = await context.Likes
                .FirstOrDefaultAsync(x => x.ProjectId == request.ProjectId && x.UserId == userId, cancellationToken);

            if (like is null)
            {
                return Result.Failure(Error.Conflict("You didn't like this project."));
            }

            context.Likes.Remove(like);

            project.LikeCount--;

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}


public class DeleteLikeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/projects/{projectId:guid}/likes", async (Guid projectId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteLike.DeleteLikeCommand(projectId));

            if (result.IsFailure)
            {
                if (result.Error == Error.NotFound("Project not found"))
                {
                    return Results.NotFound(result.Error);
                }

                if (result.Error == Error.Conflict("You didn't like this project."))
                {
                    return Results.Conflict(result.Error);
                }
            }

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Likes");
    }
}