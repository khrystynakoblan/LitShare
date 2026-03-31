namespace LitShare.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ComplaintViewModel
    {
        public int PostId { get; set; }

        [Required(ErrorMessage = "Оберіть тип проблеми")]
        public string ComplaintType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? AdditionalText { get; set; }
    }
}