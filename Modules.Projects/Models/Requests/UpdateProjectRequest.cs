using Modules.Projects.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Models.Requests;
public class UpdateProjectRequest
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public List<PhotoDto> Photos { get; set; } = [];
}
