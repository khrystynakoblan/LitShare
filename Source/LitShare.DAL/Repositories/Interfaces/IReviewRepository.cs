namespace LitShare.DAL.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.DAL.Models;

    public interface IReviewRepository
    {
        Task AddAsync(Reviews review);

        Task SaveChangesAsync();

        Task<IEnumerable<Reviews>> GetByReviewedUserIdAsync(int reviewedUserId);

        Task<bool> ExistsAsync(int reviewerId, int reviewedUserId);

        Task<Reviews?> GetByIdAsync(int id);

        Task UpdateAsync(Reviews review);
    }
}