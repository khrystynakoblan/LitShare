namespace LitShare.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ReviewViewModel
    {
        public int ReviewedUserId { get; set; }

        public string ReviewedUserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть оцінку")]
        [Range(1, 5, ErrorMessage = "Оцінка має бути від 1 до 5")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Text { get; set; }
    }
}