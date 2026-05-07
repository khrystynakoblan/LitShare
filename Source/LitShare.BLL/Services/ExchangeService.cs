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
        private readonly INotificationRepository notificationRepository;
        private readonly ILogger<ExchangeService> logger;

        public ExchangeService(IExchangeRepository exchangeRepository, IPostRepository postRepository, INotificationRepository notificationRepository, ILogger<ExchangeService> logger)
        {
            this.exchangeRepository = exchangeRepository;
            this.postRepository = postRepository;
            this.notificationRepository = notificationRepository;
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

            var notification = new Notifications
            {
                UserId = post.UserId, // Той, кому належить книга
                Message = $"Ви отримали новий запит на обмін книги '{post.Title}'.",
                IsSent = false
            };
            await this.notificationRepository.AddAsync(notification);

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

            string statusText = newStatus == RequestStatus.Accepted ? "прийнято" : "відхилено";
            var notification = new Notifications
            {
                UserId = request.SenderId,
                Message = $"Ваш запит на обмін книги '{request.Post.Title}' було {statusText}.",
                IsSent = false
            };
            await this.notificationRepository.AddAsync(notification);

            await this.exchangeRepository.SaveChangesAsync();

            this.logger.LogInformation("Request {RequestId} successfully updated to {Status}", requestId, newStatus);
            return true;
        }

        public async Task<Result<bool>> CompleteDealAsync(int requestId, int ownerId)
        {
            this.logger.LogInformation(
                "Attempting to complete deal. RequestId: {RequestId}, OwnerId: {OwnerId}",
                requestId,
                ownerId);

            var request = await this.exchangeRepository.GetByIdAsync(requestId);

            if (request == null)
            {
                return Result<bool>.Failure("Запит не знайдено.");
            }

            if (request.Post == null)
            {
                return Result<bool>.Failure("Оголошення не знайдено.");
            }

            if (request.Post.UserId != ownerId)
            {
                return Result<bool>.Failure("Ви не маєте прав для цієї дії.");
            }

            if (request.Status != RequestStatus.Accepted)
            {
                return Result<bool>.Failure("Можна завершити тільки прийнятий запит.");
            }

            request.Status = RequestStatus.Completed;

            var post = await this.postRepository.GetByIdAsync(request.PostId);

            if (post == null)
            {
                return Result<bool>.Failure("Оголошення не знайдено.");
            }

            post.IsActive = false;
            await this.exchangeRepository.SaveChangesAsync();

            return true;
        }
    }
}