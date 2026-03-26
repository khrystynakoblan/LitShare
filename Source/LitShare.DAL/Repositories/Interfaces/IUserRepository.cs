namespace LitShare.DAL.Repositories.Interfaces
{
    using LitShare.DAL.Models;

    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);

        Task AddAsync(Users user);

        Task<Users?> GetByIdAsync(int id);

        Task<Users?> GetByEmailAsync(string email);

        Task UpdateAsync(Users user);
    }
}