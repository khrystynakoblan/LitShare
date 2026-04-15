using LitShare.BLL.Common;
using LitShare.BLL.DTOs;

namespace LitShare.BLL.Services.Interfaces
{
    public interface IExchangeService
    {
        Task<Result<bool>> CreateRequestAsync(int postId, int senderId);

        Task<IEnumerable<SentRequestDto>> GetSentRequestsAsync(int senderId);
    }
}
