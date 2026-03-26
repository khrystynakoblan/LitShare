namespace LitShare.BLL.DTOs
{
    using LitShare.DAL.Models;

    public class PostFilterDto
    {
        public string? SearchTerm { get; set; }

        public string? Location { get; set; }

        public DealType? DealType { get; set; }

        public List<int>? GenreIds { get; set; }
    }
}