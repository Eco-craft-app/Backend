using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Models.DTOs;
public class CommentDto
{
    public Guid CommentId { get; set; }
    public string UserId { get; set; } = default!;
    public string UserAvatarUrl { get; set; } = default!;
    public string UserName { get; set; } = default!;

    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
