using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class oauth_consent
{
    public Guid id { get; set; }

    public Guid user_id { get; set; }

    public Guid client_id { get; set; }

    public string scopes { get; set; } = null!;

    public DateTime granted_at { get; set; }

    public DateTime? revoked_at { get; set; }

    public virtual oauth_client client { get; set; } = null!;

    public virtual user1 user { get; set; } = null!;
}
