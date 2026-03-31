namespace LitShare.BLL.DTOs
{
    public class CreateComplaintDto
    {
        public int PostId { get; set; }

        public string ComplaintType { get; set; } = string.Empty;

        public string? AdditionalText { get; set; }
    }
}