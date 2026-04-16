using LitShare.DAL.Models;

namespace LitShare.DAL.Repositories.Interfaces
{
    public interface IExchangeRepository
    {
        Task AddAsync(ExchangeRequest request);

        Task<bool> ExistsAsync(int senderId, int postId);

        Task SaveChangesAsync();

        Task<IEnumerable<ExchangeRequest>> GetBySenderIdAsync(int senderId);
    }
}
