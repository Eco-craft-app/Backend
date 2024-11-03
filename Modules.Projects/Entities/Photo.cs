using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Projects.Entities;
internal class Photo
{
    public Guid PhotoId { get; set; }
    public Guid ProjectId { get; set; }
    public string Url { get; set; } = default!;
    public bool IsMain { get; set; }
    public DateTime UploadedAt { get; set; }

    public Project? Project { get; set; }

}
