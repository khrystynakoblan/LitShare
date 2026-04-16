using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitShare.BLL.DTOs
{
    public class SentRequestDto
    {
        public int RequestId { get; set; }

        public int PostId { get; set; }

        public string BookTitle { get; set; } = string.Empty;

        public string BookAuthor { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
