namespace LitShare.BLL.Services.Interfaces
{
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IEditPostService
    {
        Task<Result<PostViewDto>> GetPostByIdAsync(int id, int currentUserId);

        Task<Result<bool>> EditPostAsync(EditPostDto dto, int currentUserId);
    }
}