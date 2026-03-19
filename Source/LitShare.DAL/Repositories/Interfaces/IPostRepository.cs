using LitShare.DAL.Models;

namespace LitShare.DAL.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Posts post);

        Task SaveChangesAsync();
    }
}