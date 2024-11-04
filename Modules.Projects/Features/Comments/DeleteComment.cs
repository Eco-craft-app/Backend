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

namespace Modules.Projects.Features.Comments;
public class DeleteComment
{
    public record DeleteCommentCommand(Guid CommentId) : IRequest<Result>;

    internal sealed class DeleteCommentHandler(ProjectsDbContext context, IUserContextService userContext) : IRequestHandler<DeleteCommentCommand, Result>
    {
        public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = userContext.GetUserId();

            var comment = await context.Comments.FirstOrDefaultAsync(c => c.CommentId == request.CommentId);

            if (comment is null)
            {
                return Result.Failure(Error.NotFound("Comment not found."));
            }

            if (comment.UserId != userId)
            {
                return Result.Failure(Error.Forbidden());
            }

            context.Comments.Remove(comment);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

public class DeleteCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("projects/{projectId:guid}/comments/{commentId:guid}", async (Guid commentId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteComment.DeleteCommentCommand(commentId));

            if (result.IsFailure)
            {
                if (result.Error == Error.Forbidden())
                {
                    return Results.Forbid();
                }

                return Results.NotFound(result.Error);
            }

            return Results.NoContent();
        })
        //.RequireAuthorization()
        .WithTags("Comments");
    }
}