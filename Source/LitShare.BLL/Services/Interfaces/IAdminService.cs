namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IAdminService
    {
        Task<Result<List<ComplaintDto>>> GetAllComplaintsAsync();

        Task<Result<ComplaintDetailsDto>> GetComplaintByIdAsync(int id);

        Task<Result<bool>> ApproveComplaintAsync(int id);

        Task<Result<bool>> RejectComplaintAsync(int id);

        Task<Result<AdminStatsDto>> GetStatisticsAsync();
    }
}