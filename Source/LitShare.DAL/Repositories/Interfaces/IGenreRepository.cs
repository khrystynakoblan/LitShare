using LitShare.DAL.Models;

namespace LitShare.DAL.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genres>> GetAllAsync();
    }
}