using System;
using System.Collections.Generic;
using System.Net;

namespace LitShare.Web.Models;

/// <summary>
/// Auth: Stores session data associated to a user.
/// </summary>
public partial class session
{
    public Guid id { get; set; }

    public Guid user_id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public Guid? factor_id { get; set; }

    /// <summary>
    /// Auth: Not after is a nullable column that contains a timestamp after which the session should be regarded as expired.
    /// </summary>
    public DateTime? not_after { get; set; }

    public DateTime? refreshed_at { get; set; }

    public string? user_agent { get; set; }

    public IPAddress? ip { get; set; }

    public string? tag { get; set; }

    public Guid? oauth_client_id { get; set; }

    /// <summary>
    /// Holds a HMAC-SHA256 key used to sign refresh tokens for this session.
    /// </summary>
    public string? refresh_token_hmac_key { get; set; }

    /// <summary>
    /// Holds the ID (counter) of the last issued refresh token.
    /// </summary>
    public long? refresh_token_counter { get; set; }

    public string? scopes { get; set; }

    public virtual ICollection<mfa_amr_claim> mfa_amr_claims { get; set; } = new List<mfa_amr_claim>();

    public virtual oauth_client? oauth_client { get; set; }

    public virtual ICollection<refresh_token> refresh_tokens { get; set; } = new List<refresh_token>();

    public virtual user1 user { get; set; } = null!;
}
