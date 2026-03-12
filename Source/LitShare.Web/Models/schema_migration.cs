using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

/// <summary>
/// Auth: Manages updates to the auth system.
/// </summary>
public partial class schema_migration
{
    public string version { get; set; } = null!;
}
