using System.Threading.Tasks;
using LitShare.BLL.DTOs;

namespace LitShare.BLL.Services.Interfaces
{
    public interface ICreatePostService
    {
        Task<int> CreatePostAsync(CreatePostDto dto, int userId);
    }
}