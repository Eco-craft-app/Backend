using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Entities;
internal class Comment
{
    public Guid CommentId { get; set; }
    public Guid ProjectId { get; set; }
    public string UserId { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public Project? Project { get; set; }

}
