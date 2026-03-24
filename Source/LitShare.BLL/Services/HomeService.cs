namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class HomeService : IHomeService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<HomeService> logger;

        public HomeService(IPostRepository postRepository, ILogger<HomeService> logger)
        {
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<Result<List<PostCardDto>>> GetAllPostsAsync()
        {
            this.logger.LogInformation("Fetching all posts for home page.");

            var posts = await this.postRepository.GetAllAsync();

            var postCards = posts.Select(p => new PostCardDto
            {
                Id = p.Id,
                Title = p.Title ?? string.Empty,
                Author = p.Author ?? string.Empty,
                City = p.User?.City,
                PhotoUrl = p.PhotoUrl,
            }).ToList();

            this.logger.LogInformation("Successfully fetched {Count} posts.", postCards.Count());

            return Result<List<PostCardDto>>.Success(postCards);
        }
    }
}