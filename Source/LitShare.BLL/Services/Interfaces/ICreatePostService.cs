namespace LitShare.BLL.Services.Interfaces
{
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;

    public interface ICreatePostService
    {
        Task<int> CreatePostAsync(CreatePostDto dto, int userId);
    }
}