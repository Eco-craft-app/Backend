using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Database;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Features.EventHandlers;
public class UsersProjectLikedHandler(UsersProfilesDbContext context) : INotificationHandler<UsersProjectLiked>
{
    public async Task Handle(UsersProjectLiked notification, CancellationToken cancellationToken)
    {
        var userProfile = await context.UsersProfiles
            .FirstOrDefaultAsync(u => u.UserId == notification.UserId, cancellationToken);

        if (userProfile is not null)
        {
            userProfile.TotalLikesReceived++;
            await context.SaveChangesAsync(cancellationToken);
        }

    }
}
