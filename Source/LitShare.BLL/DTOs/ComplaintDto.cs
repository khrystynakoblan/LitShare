namespace LitShare.BLL.DTOs
{
    public class ComplaintDto
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string BookTitle { get; set; } = string.Empty;

        public string ComplainantName { get; set; } = string.Empty;

        public DateTime Date { get; set; }
    }
}