using System;
using System.Collections.Generic;

namespace LitShare.Web.Models;

public partial class buckets_vector
{
    public string id { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<vector_index> vector_indices { get; set; } = new List<vector_index>();
}
