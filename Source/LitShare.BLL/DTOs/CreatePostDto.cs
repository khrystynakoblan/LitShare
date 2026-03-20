namespace LitShare.BLL.DTOs
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;

    public class CreatePostDto
    {
        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public List<int> GenreIds { get; set; } = new List<int>();

        public int DealTypeId { get; set; }

        public string Description { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }
    }
}