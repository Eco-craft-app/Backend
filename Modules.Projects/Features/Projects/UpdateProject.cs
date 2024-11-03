using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Modules.Projects.Entities;
using Modules.Projects.Models.DTOs;
using Modules.Projects.Models.Requests;
using Modules.Projects.Services;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.Projects;
public class UpdateProject
{
    public record UpdateProjectCommand(Guid ProjectId, string Title, string Description, List<PhotoDto> Photos) : IRequest<Result<ProjectDto>>;

    public class Validator : AbstractValidator<UpdateProjectCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleForEach(x => x.Photos).ChildRules(photo =>
            {
                photo.RuleFor(p => p.Url).Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("Photo URL must be a valid URL.");
            });
        }
    }


    internal sealed class UpdateProjectHandler(ProjectsDbContext context, IValidator<UpdateProjectCommand> validator, IUserContextService userContext) : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure<ProjectDto>(Error.Validation(validationResult.ToString()));
            }

            var userId = userContext.GetUserId();

            var project = await context.Projects.Include(p => p.Photos).FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId);

            if (project is null)
            {
                return Result.Failure<ProjectDto>(Error.NotFound("Project not found"));
            }

            if (project.UserId != userId)
            {
                return Result.Failure<ProjectDto>(Error.Forbidden());
            }

            project.Title = request.Title;
            project.Description = request.Description;
            project.Photos = request.Photos.Adapt<List<Photo>>();

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(project.Adapt<ProjectDto>());
        }
    }
}


public class UpdateProjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/projects/{projectId:guid}", async (Guid projectId, UpdateProjectRequest request, ISender sender) =>
        {
            var command = new UpdateProject.UpdateProjectCommand(projectId, request.Title, request.Description, request.Photos);

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                if (result.Error == Error.Forbidden())
                {
                    return Results.Forbid();
                }

                return Results.BadRequest(result.Error);

            }

            return Results.Ok(result.Value);

        })
        //.RequireAuthorization()
        .WithTags("Projects"); ;
    }
}