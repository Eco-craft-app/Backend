using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Models.DTOs;
internal class UserProfileDto
{
    public Guid Id { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string Location { get; set; } = string.Empty;
    public int TotalProjects { get; set; }
    public int TotalLikesReceived { get; set; }
}
