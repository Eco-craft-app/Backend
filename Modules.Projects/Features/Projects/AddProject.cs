using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Projects.Database;
using Modules.Projects.Entities;
using Modules.Projects.Models.DTOs;
using Modules.Projects.Services;
using Shared.Events;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.Projects;
public class AddProject
{
    public record AddProjectCommand(string Title, string Description, List<PhotoDto> Photos) : IRequest<Result<ProjectDto>>;

    public class Validator : AbstractValidator<AddProjectCommand>
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

    internal sealed class AddProjectHandler(ProjectsDbContext context, IValidator<AddProjectCommand> validator, IUserContextService userContext, IPublisher publisher) : IRequestHandler<AddProjectCommand, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(AddProjectCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure<ProjectDto>(Error.Validation(validationResult.ToString()));
            }

            var userId = userContext.GetUserId();

            var project = new Project
            {
                Title = request.Title,
                Description = request.Description,
                Photos = request.Photos.Adapt<List<Photo>>(),
                UserId = userId!
            };

            context.Projects.Add(project);

            await context.SaveChangesAsync(cancellationToken);

            await publisher.Publish(new ProjectCreated(project.ProjectId, Guid.Parse(project.UserId)), cancellationToken);

            return Result.Success(project.Adapt<ProjectDto>());
        }
    }
}



public class AddProjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/projects/", async (AddProject.AddProjectCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if(result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Created("api/projects", result.Value);
        })
        .RequireAuthorization()
        .WithTags("Projects");
    }
}