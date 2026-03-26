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
                dealTypeStrings = new List<string>
                {
                    filter.DealType.Value == DealType.Exchange ? "exchange" : "donation"
                };
            }

            var posts = await this.postRepository.GetFilteredAsync(
                filter.SearchTerm,
                filter.Location,
                filter.GenreIds,
                dealTypeStrings);

            var postCards = posts.Select(p => new PostCardDto
            {
                Id = p.Id,
                Title = p.Title ?? string.Empty,
                Author = p.Author ?? string.Empty,
                City = p.User?.City,
                DealType = p.DealType,
                PhotoUrl = p.PhotoUrl,
            }).ToList();

            this.logger.LogInformation("Filtered posts returned {Count} results.", postCards.Count);
            return Result<List<PostCardDto>>.Success(postCards);
        }
    }
}