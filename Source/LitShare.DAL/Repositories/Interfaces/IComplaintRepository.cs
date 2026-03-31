namespace LitShare.DAL.Repositories.Interfaces
{
    using LitShare.DAL.Models;

    public interface IComplaintRepository
    {
        Task<IEnumerable<Complaints>> GetAllAsync();
    }
}