namespace LitShare.BLL.Services.Interfaces
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IAdminService
    {
        Task<Result<List<ComplaintDto>>> GetAllComplaintsAsync();
    }
}