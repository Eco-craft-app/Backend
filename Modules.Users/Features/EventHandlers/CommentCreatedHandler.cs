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
public class CommentCreatedHandler(UsersProfilesDbContext context, IPublisher publisher) : INotificationHandler<CommentCreated>
{
    public async Task Handle(CommentCreated notification, CancellationToken cancellationToken)
    {
        var userProfile = await context.UsersProfiles
            .Where(u => u.UserId == notification.UserId)
            .Select(u => new { u.UserName, u.AvatarUrl })
            .FirstOrDefaultAsync(cancellationToken);

        if (userProfile is not null)
        {
            await publisher.Publish(new UserCommentInfoProvided(notification.CommentId, userProfile.UserName, userProfile.AvatarUrl), cancellationToken);
        }
    }
}
