namespace LitShare.DAL.Repositories.Interfaces
{
    using LitShare.DAL.Models;

    public interface IFavoriteRepository
    {
        Task<IEnumerable<Favorites>> GetByUserIdAsync(int userId);

        Task<bool> ExistsAsync(int userId, int postId);

        Task AddAsync(Favorites favorite);

        Task RemoveAsync(Favorites favorite);

        Task<Favorites?> GetAsync(int userId, int postId);

        Task<HashSet<int>> GetFavoritePostIdsAsync(int userId);

        Task SaveChangesAsync();
    }
}