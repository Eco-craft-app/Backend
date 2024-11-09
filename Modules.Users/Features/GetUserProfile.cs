using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Database;
using Modules.Users.Models.DTOs;
using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Features;
public class GetUserProfile
{
    public record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;

    internal sealed class Handler(UsersProfilesDbContext context) : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userProfile = await context.UsersProfiles
                .Where(up => up.Id == request.UserId)
                .Select(up => new UserProfileDto
                {
                    Id = up.Id,
                    AvatarUrl = up.AvatarUrl,
                    Bio = up.Bio,
                    Location = up.Location,
                    TotalProjects = up.TotalProjects,
                    TotalLikesReceived = up.TotalLikesReceived
                })
                .SingleOrDefaultAsync(cancellationToken);

            if(userProfile is null)
            {
                return Result.Failure<UserProfileDto>(Error.NotFound("There is no user's profile."));
            }

            return Result.Success(userProfile);
        }
    }
}


public class GetUsersProfileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)    
    {
        app.MapGet("api/users/{userId:guid}", async (Guid userId, ISender sender) =>
        {
            var result = await sender.Send(new GetUserProfile.GetUserProfileQuery(userId));

            if(result.IsFailure)
            {
                return Results.BadRequest(result.Error.Message);
            }

            return Results.Ok(result.Value);
        })
        .WithTags("User Profile");
    }
}
