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
public class UserCommentInfoProvidedHandler(ProjectsDbContext context) : INotificationHandler<UserCommentInfoProvided>
{
    public async Task Handle(UserCommentInfoProvided notification, CancellationToken cancellationToken)
    {
        var comment = await context.Comments
            .Where(c => c.CommentId == notification.CommentId)
            .FirstOrDefaultAsync();

        if (comment is not null)
        {
            comment.UserName = notification.UserName;
            comment.UserAvatarUrl = notification.UserAvatarUrl;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
