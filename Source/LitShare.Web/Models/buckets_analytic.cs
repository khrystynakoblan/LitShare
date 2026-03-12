using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class buckets_analytic
{
    public string name { get; set; } = null!;

    public string format { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public Guid id { get; set; }

    public DateTime? deleted_at { get; set; }
}
