namespace LitShare.BLL.DTOs
{
    public class CreateReviewDto
    {
        public int ReviewedUserId { get; set; }

        public int Rating { get; set; }

        public string? Text { get; set; }
    }
}