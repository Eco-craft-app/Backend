using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Modules.Projects.Entities;
using Modules.Projects.Services;
using Shared.Events;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.Likes;
public class AddLike
{
    public record AddLikeCommand(Guid ProjectId) : IRequest<Result>;

    internal sealed class Handler(ProjectsDbContext context, IUserContextService userContextService, IPublisher publisher) : IRequestHandler<AddLikeCommand, Result>
    {
        public async Task<Result> Handle(AddLikeCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .FirstOrDefaultAsync(x => x.ProjectId == request.ProjectId);

            if (project is null)
            {
                return Result.Failure(Error.NotFound("Project not found"));
            }

            var userId = userContextService.GetUserId();

            var userAlreadyLiked = await context.Likes
                .AnyAsync(x => x.ProjectId == request.ProjectId && x.UserId == userId, cancellationToken);
            if (userAlreadyLiked)
            {
                return Result.Failure(Error.Conflict("You already liked this project."));
            }

            var like = new Like
            {
                ProjectId = request.ProjectId,
                UserId = userId!,
                CreatedAt = DateTime.UtcNow
            };

            context.Likes.Add(like);

            project.LikeCount++;

            await context.SaveChangesAsync(cancellationToken);

            await publisher.Publish(new UsersProjectLiked(Guid.Parse(project.UserId)),cancellationToken);

            return Result.Success();

        }
    }
}


public class AddLikeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/projects/{projectId:guid}/likes", async (Guid projectId, ISender sender) =>
        {
            var result = await sender.Send(new AddLike.AddLikeCommand(projectId));

            if (result.IsFailure)
            {
                if (result.Error == Error.NotFound("Project not found"))
                {
                    return Results.NotFound(result.Error);
                }

                if (result.Error == Error.Conflict("You already liked this project."))
                {
                    return Results.Conflict(result.Error);
                }
            }

            return Results.Created();
        })
        .RequireAuthorization()
        .WithTags("Likes");
    }
}
