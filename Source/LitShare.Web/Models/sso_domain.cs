using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

/// <summary>
/// Auth: Manages SSO email address domain mapping to an SSO Identity Provider.
/// </summary>
public partial class sso_domain
{
    public Guid id { get; set; }

    public Guid sso_provider_id { get; set; }

    public string domain { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual sso_provider sso_provider { get; set; } = null!;
}
