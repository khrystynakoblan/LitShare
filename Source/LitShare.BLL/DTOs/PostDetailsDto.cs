namespace LitShare.BLL.DTOs
{
    using System.Collections.Generic;
    using LitShare.DAL.Models;

    public class PostDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? PhotoUrl { get; set; }

        public DealType DealType { get; set; }

        public List<string> Genres { get; set; } = new List<string>();

        public int UserId { get; set; }
    }
}