using LitShare.BLL.Common;
using LitShare.BLL.DTOs;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace LitShare.BLL.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository exchangeRepository;
        private readonly IPostRepository postRepository;
        private readonly ILogger<ExchangeService> logger;

        public ExchangeService(IExchangeRepository exchangeRepository, IPostRepository postRepository, ILogger<ExchangeService> logger)
        {
            this.exchangeRepository = exchangeRepository;
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<Result<bool>> CreateRequestAsync(int postId, int senderId)
        {
            this.logger.LogInformation("Attempting to create exchange request. PostId: {PostId}, SenderId: {SenderId}", postId, senderId);

            var post = await this.postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                this.logger.LogWarning("CreateRequest failed: Post not found. PostId: {PostId}", postId);
                return Result<bool>.Failure("Оголошення не знайдено.");
            }

            if (post.UserId == senderId)
            {
                this.logger.LogWarning("CreateRequest failed: User {SenderId} tried to request their own post {PostId}", senderId, postId);
                return Result<bool>.Failure("Ви не можете надіслати запит на власне оголошення.");
            }

            if (await this.exchangeRepository.ExistsAsync(senderId, postId))
            {
                this.logger.LogWarning("CreateRequest failed: Duplicate request. SenderId: {SenderId}, PostId: {PostId}", senderId, postId);
                return Result<bool>.Failure("Ви вже надсилали запит на це оголошення.");
            }

            var request = new ExchangeRequest
            {
                SenderId = senderId,
                PostId = postId,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await this.exchangeRepository.AddAsync(request);
            await this.exchangeRepository.SaveChangesAsync();

            this.logger.LogInformation("Exchange request created successfully. RequestId: {RequestId}, PostId: {PostId}", request.Id, postId);
            return true;
        }

        public async Task<IEnumerable<SentRequestDto>> GetSentRequestsAsync(int senderId)
        {
            var requests = await this.exchangeRepository.GetBySenderIdAsync(senderId);

            return requests.Select(r => new SentRequestDto
            {
                RequestId = r.Id,
                PostId = r.PostId,
                BookTitle = r.Post.Title ?? "Без назви",
                BookAuthor = r.Post.Author ?? "Невідомий автор",
                Status = r.Status.ToDisplay(),
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<Result<List<ReceivedRequestDto>>> GetReceivedRequestsAsync(int userId)
        {
            this.logger.LogInformation("Fetching received exchange requests for UserId: {UserId}", userId);

            var requests = await this.exchangeRepository.GetReceivedRequestsAsync(userId);

            var result = requests.Select(r => new ReceivedRequestDto
            {
                Id = r.Id,
                SenderId = r.SenderId,
                SenderName = r.Sender?.Name ?? "Невідомий користувач",
                PostId = r.PostId,
                PostTitle = r.Post?.Title ?? "Без назви",
                Status = r.Status.ToDisplay()
            }).ToList();

            this.logger.LogInformation("Successfully fetched {Count} received requests for UserId: {UserId}", result.Count, userId);

            return result;
        }

        public async Task<Result<bool>> UpdateRequestStatusAsync(int requestId, int ownerId, RequestStatus newStatus)
        {
            this.logger.LogInformation("Attempting to update request {RequestId} to status {Status} by User {OwnerId}", requestId, newStatus, ownerId);

            var request = await this.exchangeRepository.GetByIdAsync(requestId);

            if (request == null)
            {
                this.logger.LogWarning("Update status failed: Request {RequestId} not found.", requestId);
                return Result<bool>.Failure("Запит не знайдено.");
            }

            if (request.Post.UserId != ownerId)
            {
                this.logger.LogWarning("User {OwnerId} tried to modify request {RequestId} they don't own.", ownerId, requestId);
                return Result<bool>.Failure("Ви не маєте прав для цієї дії.");
            }

            if (request.Status != RequestStatus.Pending)
            {
                return Result<bool>.Failure("Цей запит вже був опрацьований.");
            }

            request.Status = newStatus;
            await this.exchangeRepository.SaveChangesAsync();

            this.logger.LogInformation("Request {RequestId} successfully updated to {Status}", requestId, newStatus);
            return true;
        }
    }
}