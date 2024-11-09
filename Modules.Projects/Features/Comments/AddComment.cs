using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

namespace Modules.Projects.Features.Comments;
public class AddComment
{
    public record AddCommentCommand(Guid ProjectId, string Content) : IRequest<Result<CommentDto>>; 

    public class Validator : AbstractValidator<AddCommentCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Content)
                .MaximumLength(500)
                .NotEmpty();
        }

    }


    internal sealed class Handler(ProjectsDbContext context, IValidator<AddCommentCommand> validator, IUserContextService userContext) : IRequestHandler<AddCommentCommand, Result<CommentDto>>
    {
        public async Task<Result<CommentDto>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure<CommentDto>(Error.Validation(validationResult.ToString()));
            }

            var project = await context.Projects.FirstOrDefaultAsync(x => x.ProjectId == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return Result.Failure<CommentDto>(Error.NotFound("Project not found"));
            }

            var userId = userContext.GetUserId();

            var comment = new Comment
            {
                ProjectId = request.ProjectId,
                Content = request.Content,
                UserId = userId!
            };

            context.Comments.Add(comment);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(comment.Adapt<CommentDto>());
        }
    }
}


public class AddCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/projects/{projectId:guid}/comments", async (Guid projectId, AddCommentRequest request, IMediator mediator) =>
        {
            var command = new AddComment.AddCommentCommand(projectId, request.Content);

            var result = await mediator.Send(command);

            if(result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result);
        })
        //.RequireAuthorization()
        .WithTags("Comments"); ;
    }
}
