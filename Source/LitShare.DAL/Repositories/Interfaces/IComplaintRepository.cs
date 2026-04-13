namespace LitShare.DAL.Repositories.Interfaces
{
    using LitShare.DAL.Models;

    public interface IComplaintRepository
    {
        Task AddAsync(Complaints complaint);

        Task SaveChangesAsync();

        Task<IEnumerable<Complaints>> GetAllAsync();

        Task<Complaints?> GetByIdAsync(int id);

        Task DeleteAsync(Complaints complaint);
    }
}