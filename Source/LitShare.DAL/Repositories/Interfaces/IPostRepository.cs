using LitShare.DAL.Models;

namespace LitShare.DAL.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Posts post);
<<<<<<< HEAD
=======

        Task SaveChangesAsync();
>>>>>>> ef4b67b (Add Create Post Window and tests)
    }
}