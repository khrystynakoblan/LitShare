namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.DAL.Models;

    public interface IProfileService
    {
        Task<Users?> GetUserByIdAsync(int id);
    }
}