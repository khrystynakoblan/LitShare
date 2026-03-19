namespace LitShare.BLL.DTOs
{
    public class EditPostDto
    {
        public int PostId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int GenreId { get; set; }

        public int DealTypeId { get; set; }
    }
}