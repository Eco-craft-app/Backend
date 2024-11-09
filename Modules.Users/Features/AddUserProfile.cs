using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Database;
using Modules.Users.Entities;
using Modules.Users.Models.DTOs;
using Modules.Users.Services;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Modules.Users.Features.AddUserProfile;

namespace Modules.Users.Features;
public class AddUserProfile
{
    public record AddUserProfileCommand(string AvatarUrl, string? Bio, string Location) : IRequest<Result<UserProfileDto>>;

    public class Validator : AbstractValidator<AddUserProfileCommand>
    {
        public Validator()
        {
            RuleFor(p => p.AvatarUrl)
                .Must(uri => Uri
                    .IsWellFormedUriString(uri, UriKind.Absolute))
                    .WithMessage("Photo URL must be a valid URL.")
                .NotEmpty();

            RuleFor(p => p.Bio)
                .MaximumLength(500);

            RuleFor(p => p.Location)
                .NotEmpty();


        }
    }

    internal sealed class Handler(UsersProfilesDbContext context, IValidator<AddUserProfileCommand> validator, IUserContextService userContext) : IRequestHandler<AddUserProfileCommand, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(AddUserProfileCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure<UserProfileDto>(Error.Validation(validationResult.ToString()));
            }

            var userId = userContext.GetUserId();

            var userProfile = new UserProfile
            {
                Id = Guid.Parse(userId!),
                AvatarUrl = request.AvatarUrl,
                Bio = request.Bio,
                Location = request.Location,
                TotalLikesReceived = 0,
                TotalProjects = 0
            };

            context.UsersProfiles.Add(userProfile);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(userProfile.Adapt<UserProfileDto>());
        }
    }

}


public class AddUserProfileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/users/profile", async (AddUserProfileCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error.Message);
            }

            return Results.Created("/users/{id}", result.Value);
        })
        .RequireAuthorization()
        .WithTags("User Profile");
    }
}