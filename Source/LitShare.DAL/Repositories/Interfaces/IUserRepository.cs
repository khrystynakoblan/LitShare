namespace LitShare.DAL.Repositories.Interfaces
{
    using LitShare.DAL.Models;

    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);

        Task AddAsync(Users user);
    }
}