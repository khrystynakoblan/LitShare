namespace LitShare.Web.Models
{
    using LitShare.BLL.DTOs;
    using LitShare.DAL.Models;

    public class HomeViewModel
    {
        public List<PostCardDto> Posts { get; set; } = new List<PostCardDto>();

        public int TotalCount => this.Posts.Count;

        public string? SearchTerm { get; set; }

        public string? Location { get; set; }

        public DealType? DealType { get; set; }

        public List<int>? SelectedGenres { get; set; }

        public List<GenreDto> AllGenres { get; set; } = new List<GenreDto>();

        public HashSet<int> FavoritePostIds { get; set; } = new HashSet<int>();
    }
}