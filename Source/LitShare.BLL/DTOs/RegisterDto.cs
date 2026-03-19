namespace LitShare.BLL.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Region { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }
    }
}