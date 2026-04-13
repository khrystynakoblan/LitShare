namespace LitShare.BLL.DTOs
{
    public class ComplaintDetailsDto
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string ComplainantName { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int PostId { get; set; }

        public string BookTitle { get; set; } = string.Empty;

        public string BookAuthor { get; set; } = string.Empty;

        public string? BookDescription { get; set; }

        public string? BookPhotoUrl { get; set; }
    }
}