using LitShare.BLL.Common;
using LitShare.BLL.DTOs;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace LitShare.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<PostService> logger;

        public PostService(IPostRepository postRepository, ILogger<PostService> logger)
        {
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<PostCardDto>>> GetPostsByUserIdAsync(int userId)
        {
            this.logger.LogInformation("BLL: Fetching posts for UserId: {UserId}", userId);

            var posts = await this.postRepository.GetByUserIdAsync(userId);

            var dtos = posts.Select(p => new PostCardDto
            {
                Id = p.Id,
                Title = p.Title ?? string.Empty,
                Author = p.Author ?? string.Empty,
                City = p.User?.City ?? "Не вказано",
                PhotoUrl = p.PhotoUrl
            });

            return Result<IEnumerable<PostCardDto>>.Success(dtos);
        }
    }
}