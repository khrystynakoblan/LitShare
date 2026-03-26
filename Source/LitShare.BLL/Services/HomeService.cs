namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
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

            this.logger.LogInformation("Successfully fetched {Count} posts.", postCards.Count);

            return Result<List<PostCardDto>>.Success(postCards);
        }

        public async Task<Result<List<PostCardDto>>> GetFilteredPostsAsync(PostFilterDto filter)
        {
            this.logger.LogInformation(
                "Fetching filtered posts. SearchQuery: {SearchQuery}, City: {City}, Genres: {GenreCount}, DealType: {DealType}",
                filter.SearchTerm,
                filter.Location,
                filter.GenreIds?.Count ?? 0,
                filter.DealType);

            List<string>? dealTypeStrings = null;
            if (filter.DealType.HasValue)
            {
                var dealTypeString = filter.DealType.Value == DealType.Exchange ? "exchange" : "donation";
                dealTypeStrings = new List<string> { dealTypeString };
            }

            var posts = await this.postRepository.GetFilteredAsync(
                filter.SearchTerm,
                filter.Location,
                filter.GenreIds,
                null);

            if (dealTypeStrings != null && dealTypeStrings.Any())
            {
                posts = posts.Where(p => dealTypeStrings.Contains(p.DealType.ToString().ToLower()));
            }

            var postCards = posts.Select(p => new PostCardDto
            {
                Id = p.Id,
                Title = p.Title ?? string.Empty,
                Author = p.Author ?? string.Empty,
                City = p.User?.City,
                PhotoUrl = p.PhotoUrl,
            }).ToList();

            this.logger.LogInformation("Filtered posts returned {Count} results.", postCards.Count);

            return Result<List<PostCardDto>>.Success(postCards);
        }
    }
}