using System.Threading.Tasks;
using LitShare.BLL.DTOs;

namespace LitShare.BLL.Services.Interfaces
{
    public interface IEditPostService
    {
        Task EditPostAsync(EditPostDto dto);
    }
}