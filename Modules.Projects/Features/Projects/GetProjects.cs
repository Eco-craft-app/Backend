using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
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

namespace Modules.Projects.Features.Projects;
public static class GetProjects
{
    public record Query() : IRequest<Result<PagedResult<ProjectSummaryDto>>>
    {
        public SieveModel Model { get; set; } = default!;
    }

    internal sealed class Handler(ProjectsDbContext context, ISieveProcessor sieveProcessor) : IRequestHandler<Query, Result<PagedResult<ProjectSummaryDto>>>
    {
        public async Task<Result<PagedResult<ProjectSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var projects = context.Projects
                .Include(p => p.Photos.Where(p => p.IsMain == true))
                .Include(p => p.Comments)
                .AsNoTracking();

            var projectSummaries = await sieveProcessor
                .Apply(request.Model, projects)
                .Select(p => new ProjectSummaryDto
                {
                    UserId = p.UserId,
                    ProjectId = p.ProjectId,
                    Title = p.Title,
                    PhotoUrl = p.Photos.FirstOrDefault()!.Url,
                    CreatedAt = p.CreatedAt,
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

public class GetProjectsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/projects", async ([AsParameters] SieveModel model, ISender sender) =>
        {
            if (model is null)
            {
                return Results.BadRequest();
            }

            var result = await sender.Send(new GetProjects.Query() { Model = model });

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