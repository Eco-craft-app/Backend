using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Database;
using Modules.Users.Models.DTOs;
using Modules.Users.Services;
using Shared.Events;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Modules.Users.Features.UpdateUserProfile;

namespace Modules.Users.Features;
public class UpdateUserProfile
{
    public record UpdateUserProfileCommand(string AvatarUrl, string UserName, string? Bio, string Location) : IRequest<Result<UserProfileDto>>;

    public class Validator : AbstractValidator<UpdateUserProfileCommand>
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

    internal sealed class Handler(UsersProfilesDbContext context, IValidator<UpdateUserProfileCommand> validator, IUserContextService userContext, IPublisher publisher) : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure<UserProfileDto>(Error.Validation(validationResult.ToString()));
            }

            var userId = userContext.GetUserId();

            var userProfile = await context.UsersProfiles.FindAsync(Guid.Parse(userId!));

            if (userProfile is null)
            {
                return Result.Failure<UserProfileDto>(Error.NotFound("User profile not found."));
            }

            userProfile.AvatarUrl = request.AvatarUrl;
            userProfile.UserName = request.UserName;
            userProfile.Bio = request.Bio;
            userProfile.Location = request.Location;

            await context.SaveChangesAsync(cancellationToken);

            await publisher.Publish(new UserProfileUpdated(userProfile.Id, userProfile.UserName, userProfile.AvatarUrl));


            return Result.Success(userProfile.Adapt<UserProfileDto>());
        }
    }
}

public class UpdateUserProfileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/users/profile", async (UpdateUserProfileCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error.Message);
            }

            return Results.Ok(result.Value);
        })
        .RequireAuthorization()
        .WithTags("User Profile");
    }
}