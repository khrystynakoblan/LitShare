using LitShare.BLL.DTOs;

namespace LitShare.Web.Models
{
    public class ProfileViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string About { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }

        public IEnumerable<PostCardDto> UserBooks { get; set; } = new List<PostCardDto>();
    }
}