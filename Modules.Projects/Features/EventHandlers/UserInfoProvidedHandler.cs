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
public class UserInfoProvidedHandler(ProjectsDbContext context) : INotificationHandler<UserInfoProvided>
{
    public async Task Handle(UserInfoProvided notification, CancellationToken cancellationToken)
    {
        var project = await context.Projects
             .Where(p => p.ProjectId == notification.ProjectId)
             .FirstOrDefaultAsync(cancellationToken);

        if (project is not null)
        {
            project.UserName = notification.UserName;
            project.UserAvatarUrl = notification.AvatarUrl;
            await context.SaveChangesAsync(cancellationToken);
        }

    }
}
