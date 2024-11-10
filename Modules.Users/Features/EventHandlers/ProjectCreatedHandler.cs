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
public class ProjectCreatedHandler(UsersProfilesDbContext context, IPublisher publisher) : INotificationHandler<ProjectCreated>
{
    public async Task Handle(ProjectCreated notification, CancellationToken cancellationToken)
    {
        var user = await context.UsersProfiles
             .Where(u => u.UserId == notification.UserId)
             .FirstOrDefaultAsync(cancellationToken);



        if (user is not null)
        {
            user.TotalProjects++;
            await context.SaveChangesAsync(cancellationToken);
            await publisher.Publish(new UserInfoProvided(notification.ProjectId, user.UserName, user.AvatarUrl), cancellationToken);
        }
    }
}
