namespace LitShare.DAL.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using LitShare.DAL.Models;

    public interface IComplaintRepository
    {
        Task AddAsync(Complaints complaint);

        Task SaveChangesAsync();

        Task<IEnumerable<Complaints>> GetAllAsync();
    }
}