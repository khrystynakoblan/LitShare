using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class subscription
{
    public long id { get; set; }

    public Guid subscription_id { get; set; }

    public string claims { get; set; } = null!;

    public DateTime created_at { get; set; }

    public string? action_filter { get; set; }
}
