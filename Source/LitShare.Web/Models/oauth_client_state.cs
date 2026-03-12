using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

/// <summary>
/// Stores OAuth states for third-party provider authentication flows where Supabase acts as the OAuth client.
/// </summary>
public partial class oauth_client_state
{
    public Guid id { get; set; }

    public string provider_type { get; set; } = null!;

    public string? code_verifier { get; set; }

    public DateTime created_at { get; set; }
}
