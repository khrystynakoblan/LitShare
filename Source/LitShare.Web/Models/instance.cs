using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

/// <summary>
/// Auth: Manages users across multiple sites.
/// </summary>
public partial class instance
{
    public Guid id { get; set; }

    public Guid? uuid { get; set; }

    public string? raw_base_config { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }
}
