using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class oauth_client
{
    public Guid id { get; set; }

    public string? client_secret_hash { get; set; }

    public string redirect_uris { get; set; } = null!;

    public string grant_types { get; set; } = null!;

    public string? client_name { get; set; }

    public string? client_uri { get; set; }

    public string? logo_uri { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public string token_endpoint_auth_method { get; set; } = null!;

    public virtual ICollection<oauth_authorization> oauth_authorizations { get; set; } = new List<oauth_authorization>();

    public virtual ICollection<oauth_consent> oauth_consents { get; set; } = new List<oauth_consent>();

    public virtual ICollection<session> sessions { get; set; } = new List<session>();
}
