namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.Common;
    using LitShare.DAL.Models;

    public interface IProfileService
    {
        Task<Result<Users>> GetUserByIdAsync(int id);
    }
}