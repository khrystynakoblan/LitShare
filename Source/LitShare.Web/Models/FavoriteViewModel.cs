namespace LitShare.Web.Models
{
    using LitShare.BLL.DTOs;

    public class FavoriteViewModel
    {
        public List<FavoriteDto> Favorites { get; set; } = new List<FavoriteDto>();

        public int TotalCount => this.Favorites.Count;
    }
}
