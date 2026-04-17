namespace LitShare.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class EditReviewViewModel
    {
        public int ReviewId { get; set; }

        public int ReviewedUserId { get; set; }

        [Required(ErrorMessage = "Оберіть оцінку")]
        [Range(1, 5, ErrorMessage = "Оцінка має бути від 1 до 5")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Text { get; set; }
    }
}