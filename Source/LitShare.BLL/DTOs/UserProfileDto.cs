using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitShare.BLL.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Region { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? About { get; set; }

        public string? PhotoUrl { get; set; }
    }
}
