namespace LitShare.Web.Models
{
    using LitShare.BLL.DTOs;

    public class HomeViewModel
    {
        public List<PostCardDto> Posts { get; set; } = new List<PostCardDto>();

        public int TotalCount => this.Posts.Count;
    }
}