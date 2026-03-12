using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

/// <summary>
/// Stores metadata for all OAuth/SSO login flows
/// </summary>
public partial class flow_state
{
    public Guid id { get; set; }

    public Guid? user_id { get; set; }

    public string? auth_code { get; set; }

    public string? code_challenge { get; set; }

    public string provider_type { get; set; } = null!;

    public string? provider_access_token { get; set; }

    public string? provider_refresh_token { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string authentication_method { get; set; } = null!;

    public DateTime? auth_code_issued_at { get; set; }

    public string? invite_token { get; set; }

    public string? referrer { get; set; }

    public Guid? oauth_client_state_id { get; set; }

    public Guid? linking_target_id { get; set; }

    public bool email_optional { get; set; }

    public virtual ICollection<saml_relay_state> saml_relay_states { get; set; } = new List<saml_relay_state>();
}
