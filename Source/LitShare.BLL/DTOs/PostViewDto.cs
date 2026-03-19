namespace LitShare.BLL.DTOs
{
    using LitShare.DAL.Models;

    public class PostViewDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DealType DealType { get; set; }

        public string? PhotoUrl { get; set; }

        public int GenreId { get; set; }
    }
}