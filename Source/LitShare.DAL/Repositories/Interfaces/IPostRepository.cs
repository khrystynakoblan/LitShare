using System.Threading.Tasks;
using LitShare.DAL.Models;

namespace LitShare.DAL.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Posts post);

        Task SaveChangesAsync();

        Task<Posts?> GetByIdAsync(int id);

        Task UpdateAsync(Posts post);
    }
}