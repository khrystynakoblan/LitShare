namespace LitShare.BLL.Services.Interfaces
{
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IComplaintService
    {
        Task<Result<bool>> CreateComplaintAsync(CreateComplaintDto dto, int userId);
    }
}