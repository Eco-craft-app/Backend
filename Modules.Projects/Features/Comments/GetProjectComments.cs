using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Modules.Projects.Entities;
using Modules.Projects.Models.DTOs;
using Modules.Projects.Models.Response;
using Shared.ResultTypes;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.Comments;
public class GetProjectComments
{
    public record Query(Guid ProjectId, SieveModel Model) : IRequest<Result<PagedResult<CommentDto>>>;

    internal sealed class Handler(ProjectsDbContext context, ISieveProcessor sieveProcessor) : IRequestHandler<Query, Result<PagedResult<CommentDto>>>
    {
        public async Task<Result<PagedResult<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var comments = context.Comments
                .Where(c => c.ProjectId == request.ProjectId)
                .AsNoTracking();

            var commentDtos = await sieveProcessor
                .Apply(request.Model, comments)
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId
                })
                .ToListAsync(cancellationToken);

            var totalCount = await sieveProcessor.Apply(request.Model, comments, applyPagination: false, applySorting: false)
                .CountAsync(cancellationToken);

            var result = new PagedResult<CommentDto>(commentDtos, totalCount, request.Model.PageSize!.Value, request.Model.Page!.Value);

            return Result.Success(result);
        }
    }
}


public class GetProjectCommentsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("projects/{projectId:guid}/comments", async (Guid projectId, [AsParameters] SieveModel model, ISender sender) =>
        {
            var result = await sender.Send(new GetProjectComments.Query(projectId, model));

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .WithTags("Comments");
    }
}
