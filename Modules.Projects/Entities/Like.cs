using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Entities;
public class Like
{
    public Guid LikeId { get; set; }
    public Guid ProjectId { get; set; }
    public string UserId { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public Project? Project { get; set; }
}
