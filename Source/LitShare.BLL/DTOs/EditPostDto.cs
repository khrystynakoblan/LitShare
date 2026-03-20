namespace LitShare.BLL.DTOs
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;

    public class EditPostDto
    {
        public int PostId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<int> GenreIds { get; set; } = new List<int>();

        public int DealTypeId { get; set; }

        public IFormFile? NewPhoto { get; set; }
    }
}