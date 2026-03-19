using System.Threading.Tasks;
using LitShare.BLL.DTOs;
using Microsoft.AspNetCore.Http;

namespace LitShare.BLL.Services.Interfaces
{
    public interface ICreatePostService
    {
        // Змінюємо Task на Task<int>
        Task<int> CreatePostAsync(CreatePostDto dto, IFormFile? imageFile, int userId);
    }
}