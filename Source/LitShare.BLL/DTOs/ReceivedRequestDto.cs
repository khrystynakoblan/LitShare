namespace LitShare.BLL.DTOs
{
    public class ReceivedRequestDto
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public string SenderName { get; set; } = string.Empty;

        public int PostId { get; set; }

        public string PostTitle { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}