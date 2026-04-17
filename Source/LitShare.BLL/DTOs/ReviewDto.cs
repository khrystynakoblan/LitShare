namespace LitShare.BLL.DTOs
{
    using System;

    public class ReviewDto
    {
        public int Id { get; set; }

        public string? Text { get; set; }

        public int Rating { get; set; }

        public DateTime Date { get; set; }

        public int ReviewerId { get; set; }

        public int ReviewedUserId { get; set; }

        public string ReviewerName { get; set; } = string.Empty;

        public string? ReviewerPhotoUrl { get; set; }
    }
}