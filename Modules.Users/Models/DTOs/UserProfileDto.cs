using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Models.DTOs;
internal class UserProfileDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string AvatarUrl { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string Location { get; set; } = string.Empty;
    public int TotalProjects { get; set; }
    public int TotalLikesReceived { get; set; }
}
