using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class post
{
    public int id { get; set; }

    public int? user_id { get; set; }

    public string title { get; set; } = null!;

    public string? author { get; set; }

    public string? description { get; set; }

    public string? photo_url { get; set; }

    public virtual ICollection<complaint> complaints { get; set; } = new List<complaint>();

    public virtual user? user { get; set; }

    public virtual ICollection<genre> genres { get; set; } = new List<genre>();
}
