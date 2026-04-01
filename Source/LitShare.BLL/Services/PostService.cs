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
            }).ToList();

            return dtos;
        }

        public async Task<Result<PostDetailsDto>> GetPostDetailsAsync(int id)
        {
            this.logger.LogInformation("BLL: Fetching post details for ID: {Id}", id);

            var post = await this.postRepository.GetByIdWithGenresAsync(id);

            if (post == null)
            {
                this.logger.LogWarning("BLL: Post with ID: {Id} was not found", id);
                return Result<PostDetailsDto>.Failure($"Post with ID {id} not found");
            }

            this.logger.LogInformation("BLL: Successfully fetched post details for ID: {Id}", id);

            return new PostDetailsDto
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Author = post.Author ?? string.Empty,
                Description = post.Description,
                PhotoUrl = post.PhotoUrl,
                DealType = post.DealType,
                Genres = post.BookGenres
                    .Select(bg => bg.Genre?.Name ?? string.Empty)
                    .ToList(),
                UserId = post.UserId,
            };
        }
    }
}