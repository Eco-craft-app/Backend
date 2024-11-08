using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Entities;
public class UserProfile
{
    public Guid Id { get; set; }
    public string AvatarUrl { get; set; } = "https://res.cloudinary.com/default-placeholder";
    public string? Bio { get; set; }
    public string Location { get; set; } = string.Empty;
    public int TotalProjects { get; set; }
    public int TotalLikesReceived { get; set; }
}