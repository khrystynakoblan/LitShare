using LitShare.BLL.Common;
using LitShare.BLL.DTOs;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;

namespace LitShare.BLL.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository exchangeRepository;
        private readonly IPostRepository postRepository;

        public ExchangeService(IExchangeRepository exchangeRepository, IPostRepository postRepository)
        {
            this.exchangeRepository = exchangeRepository;
            this.postRepository = postRepository;
        }

        public async Task<Result<bool>> CreateRequestAsync(int postId, int senderId)
        {
            var post = await this.postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                return Result<bool>.Failure("Оголошення не знайдено.");
            }

            if (post.UserId == senderId)
            {
                return Result<bool>.Failure("Ви не можете надіслати запит на власне оголошення.");
            }

            if (await this.exchangeRepository.ExistsAsync(senderId, postId))
            {
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