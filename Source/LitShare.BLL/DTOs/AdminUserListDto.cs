using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitShare.BLL.DTOs
{
    public class AdminUserListDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
