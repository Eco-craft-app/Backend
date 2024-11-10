using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Database;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Features.EventHandlers;
public class UserProfileUpdatedHandler(ProjectsDbContext context) : INotificationHandler<UserProfileUpdated>
{
    public async Task Handle(UserProfileUpdated notification, CancellationToken cancellationToken)
    {
        var projects = await context.Projects
            .Where(p => p.UserId == notification.UserId.ToString())
            .ToListAsync(cancellationToken);

        var comments = await context.Comments
            .Where(c => c.UserId == notification.UserId.ToString())
            .ToListAsync(cancellationToken);

        foreach (var project in projects)
        {
            project.UserAvatarUrl = notification.AvatarUrl;
        }

        foreach (var comment in comments)
        {
            comment.UserAvatarUrl = notification.AvatarUrl;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
