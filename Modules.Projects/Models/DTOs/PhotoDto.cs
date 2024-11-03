using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Models.DTOs;
public class PhotoDto
{
    public string Url { get; set; } = default!;
    public bool IsMain { get; set; }
}
