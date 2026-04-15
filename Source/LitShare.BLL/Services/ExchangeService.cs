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
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt
            });
        }
    }
}