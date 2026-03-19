namespace LitShare.BLL.DTOs
{
    public class CreatePostDto
    {
        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public int GenreId { get; set; }

        public int DealTypeId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }
    }
}