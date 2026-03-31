namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.DAL.Models;

    public interface IProfileService
    {
        Task<Result<Users>> GetUserByIdAsync(int id);

        Task<Result<bool>> UpdateProfileAsync(int userId, UpdateProfileDto dto);

        Task<Result<bool>> DeleteAccountAsync(int userId);
    }
}