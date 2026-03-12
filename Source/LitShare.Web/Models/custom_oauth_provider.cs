using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class custom_oauth_provider
{
    public Guid id { get; set; }

    public string provider_type { get; set; } = null!;

    public string identifier { get; set; } = null!;

    public string name { get; set; } = null!;

    public string client_id { get; set; } = null!;

    public string client_secret { get; set; } = null!;

    public List<string> acceptable_client_ids { get; set; } = null!;

    public List<string> scopes { get; set; } = null!;

    public bool pkce_enabled { get; set; }

    public string attribute_mapping { get; set; } = null!;

    public string authorization_params { get; set; } = null!;

    public bool enabled { get; set; }

    public bool email_optional { get; set; }

    public string? issuer { get; set; }

    public string? discovery_url { get; set; }

    public bool skip_nonce_check { get; set; }

    public string? cached_discovery { get; set; }

    public DateTime? discovery_cached_at { get; set; }

    public string? authorization_url { get; set; }

    public string? token_url { get; set; }

    public string? userinfo_url { get; set; }

    public string? jwks_uri { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }
}
