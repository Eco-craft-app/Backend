using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Entities;
public class Comment
{
    public Guid CommentId { get; set; }
    public Guid ProjectId { get; set; }
    public string UserId { get; set; } = default!;
    public string? UserAvatarUrl { get; set; }
    public string? UserName { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project? Project { get; set; }

}
