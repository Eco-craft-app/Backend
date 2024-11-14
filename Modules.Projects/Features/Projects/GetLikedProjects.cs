using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Modules.Projects.Models.DTOs;
using Modules.Projects.Models.Response;
using Modules.Projects.Services;
using Shared.ResultTypes;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.Projects;
public class GetLikedProjects
{
    public record Query(SieveModel Model) : IRequest<Result<PagedResult<ProjectSummaryDto>>>;

    internal sealed class Handler(ProjectsDbContext context, IUserContextService userContext, ISieveProcessor sieveProcessor) : IRequestHandler<Query, Result<PagedResult<ProjectSummaryDto>>>
    {
        public async Task<Result<PagedResult<ProjectSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = userContext.GetUserId();

            var projects = context.Projects
                .Include(p => p.Photos.Where(p => p.IsMain))
                .Where(p => p.Likes.Any(l => l.UserId == userId))
                .AsNoTracking();

            var projectSummaries = await sieveProcessor
                .Apply(request.Model, projects)
                .Select(p => new ProjectSummaryDto
                {
                    ProjectId = p.ProjectId,
                    Title = p.Title,
                    PhotoUrl = p.Photos.FirstOrDefault(p => p.IsMain == true)!.Url,
                    CreatedAt = p.CreatedAt,
                    UserAvatarUrl = p.UserAvatarUrl!,
                    UserId = p.UserId,
                    UserName = p.UserName!,
                    IsLikedByCurrentUser = true,
                    LikeCount = p.LikeCount
                })
                .ToListAsync(cancellationToken);

            var totalCount = await sieveProcessor.Apply(request.Model, projects, applyPagination: false, applySorting: false)
                      .CountAsync(cancellationToken);

            var result = new PagedResult<ProjectSummaryDto>(projectSummaries, totalCount, request.Model.PageSize!.Value, request.Model.Page!.Value);

            return Result.Success(result);
        }
    }
}


public class GetLikedProjectsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/projects/liked", async ([AsParameters] SieveModel model, ISender sender) =>
        {
            if (model is null)
            {
                return Results.BadRequest();
            }

            var result = await sender.Send(new GetLikedProjects.Query(model));

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        })
        //.RequireAuthorization()
        .WithTags("Projects");
    }
}