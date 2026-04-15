using LitShare.BLL.Common;
using LitShare.BLL.DTOs;

namespace LitShare.BLL.Services.Interfaces
{
    public interface IPostService
    {
        Task<Result<IEnumerable<PostCardDto>>> GetPostsByUserIdAsync(int userId, bool? isActive);

        Task<Result<PostDetailsDto>> GetPostDetailsAsync(int id);

        Task<Result<bool>> SetPostStatusAsync(int postId, int userId, bool isActive);

        Task<Result<bool>> DeletePostAsync(int postId, int userId);
    }
}