using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class genre
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<post> posts { get; set; } = new List<post>();
}
