namespace LitShare.BLL.DTOs
{
    public class PostCardDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? PhotoUrl { get; set; }
    }
}