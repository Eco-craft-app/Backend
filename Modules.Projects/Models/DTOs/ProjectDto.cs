using Modules.Projects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Models.DTOs;
public class ProjectDto
{
    public Guid ProjectId { get; set; }
    public string UserId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int LikeCount { get; set; }   
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PhotoDto> Photos { get; set; } = [];

}
