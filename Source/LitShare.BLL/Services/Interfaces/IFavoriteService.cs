namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IFavoriteService
    {
        Task<Result<List<FavoriteDto>>> GetFavoritesAsync(int userId);

        Task<Result<bool>> AddToFavoritesAsync(int userId, int postId);

        Task<Result<bool>> RemoveFromFavoritesAsync(int userId, int postId);

        Task<Result<HashSet<int>>> GetFavoritePostIdsAsync(int userId);
    }
}