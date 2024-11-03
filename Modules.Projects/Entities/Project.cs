using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Entities;
internal class Project
{
    public Guid ProjectId { get; set; }

    [Sieve(CanFilter = true)]
    public string UserId { get; set; } = default!;

    [Sieve(CanFilter = true)]
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;

    [Sieve(CanSort = true)]
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Photo> Photos { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Like> Likes { get; set; } = [];
}
