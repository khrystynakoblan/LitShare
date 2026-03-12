using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class vector_index
{
    public string id { get; set; } = null!;

    public string name { get; set; } = null!;

    public string bucket_id { get; set; } = null!;

    public string data_type { get; set; } = null!;

    public int dimension { get; set; }

    public string distance_metric { get; set; } = null!;

    public string? metadata_configuration { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual buckets_vector bucket { get; set; } = null!;
}
