using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class user
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string email { get; set; } = null!;

    public string? phone { get; set; }

    public string password { get; set; } = null!;

    public string? about { get; set; }

    public string? region { get; set; }

    public string? district { get; set; }

    public string? city { get; set; }

    public string? photo_url { get; set; }

    public virtual ICollection<complaint> complaints { get; set; } = new List<complaint>();

    public virtual ICollection<post> posts { get; set; } = new List<post>();
}
