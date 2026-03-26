namespace LitShare.BLL.DTOs
{
    public class UpdateProfileDto
    {
        public string Email { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string? Phone { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string About { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }
    }
}