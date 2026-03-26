using LitShare.BLL.Common;
using LitShare.BLL.DTOs;

namespace LitShare.BLL.Services.Interfaces
{
    public interface IPostService
    {
        Task<Result<IEnumerable<PostCardDto>>> GetPostsByUserIdAsync(int userId);

        Task<Result<PostDetailsDto>> GetPostDetailsAsync(int id);
    }
}