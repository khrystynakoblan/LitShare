namespace LitShare.DAL.Repositories.Interfaces
{
    using LitShare.DAL.Models;

    public interface IPostRepository
    {
        Task<IEnumerable<Posts>> GetAllAsync();

        Task<IEnumerable<Posts>> GetFilteredAsync(
            string? searchQuery,
            string? city,
            List<int>? genreIds,
            List<string>? dealTypeStrings);

        Task AddAsync(Posts post);

        Task SaveChangesAsync();

        Task<Posts?> GetByIdAsync(int id);

        Task<IEnumerable<Posts>> GetByUserIdAsync(int userId);

        Task UpdateAsync(Posts post);

        Task<Posts?> GetByIdWithGenresAsync(int id);
    }
}