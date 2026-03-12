using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class oauth_authorization
{
    public Guid id { get; set; }

    public string authorization_id { get; set; } = null!;

    public Guid client_id { get; set; }

    public Guid? user_id { get; set; }

    public string redirect_uri { get; set; } = null!;

    public string scope { get; set; } = null!;

    public string? state { get; set; }

    public string? resource { get; set; }

    public string? code_challenge { get; set; }

    public string? authorization_code { get; set; }

    public DateTime created_at { get; set; }

    public DateTime expires_at { get; set; }

    public DateTime? approved_at { get; set; }

    public string? nonce { get; set; }

    public virtual oauth_client client { get; set; } = null!;

    public virtual user1? user { get; set; }
}
