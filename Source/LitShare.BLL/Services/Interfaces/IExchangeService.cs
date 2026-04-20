using LitShare.BLL.Common;
using LitShare.BLL.DTOs;
using LitShare.DAL.Models;

namespace LitShare.BLL.Services.Interfaces
{
    public interface IExchangeService
    {
        Task<Result<bool>> CreateRequestAsync(int postId, int senderId);

        Task<IEnumerable<SentRequestDto>> GetSentRequestsAsync(int senderId);

        Task<Result<List<ReceivedRequestDto>>> GetReceivedRequestsAsync(int userId);

        Task<Result<bool>> UpdateRequestStatusAsync(int requestId, int ownerId, RequestStatus newStatus);
    }
}
