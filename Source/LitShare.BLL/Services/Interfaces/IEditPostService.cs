namespace LitShare.BLL.Services.Interfaces
{
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;

    public interface IEditPostService
    {
        Task<PostViewDto?> GetPostByIdAsync(int id);

        Task EditPostAsync(EditPostDto dto);
    }
}