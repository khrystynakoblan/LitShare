using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class complaint
{
    public int id { get; set; }

    public string text { get; set; } = null!;

    public DateTime? date { get; set; }

    public int? post_id { get; set; }

    public int? complainant_id { get; set; }

    public virtual user? complainant { get; set; }

    public virtual post? post { get; set; }
}
