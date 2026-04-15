namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository favoriteRepository;
        private readonly ILogger<FavoriteService> logger;

        public FavoriteService(IFavoriteRepository favoriteRepository, ILogger<FavoriteService> logger)
        {
            this.favoriteRepository = favoriteRepository;
            this.logger = logger;
        }

        public async Task<Result<List<FavoriteDto>>> GetFavoritesAsync(int userId)
        {
            this.logger.LogInformation("Fetching favorites for user {UserId}.", userId);

            var favorites = await this.favoriteRepository.GetByUserIdAsync(userId);

            var dtos = favorites
                .Where(f => f.Post != null)
                .Select(f => new FavoriteDto
                {
                    PostId = f.PostId,
                    Title = f.Post!.Title ?? string.Empty,
                    Author = f.Post.Author ?? string.Empty,
                    City = f.Post.User?.City,
                    PhotoUrl = f.Post.PhotoUrl,
                }).ToList();

            this.logger.LogInformation("Fetched {Count} favorites for user {UserId}.", dtos.Count, userId);

            return dtos;
        }

        public async Task<Result<bool>> AddToFavoritesAsync(int userId, int postId)
        {
            this.logger.LogInformation("User {UserId} adding post {PostId} to favorites.", userId, postId);

            bool alreadyExists = await this.favoriteRepository.ExistsAsync(userId, postId);

            if (alreadyExists)
            {
                this.logger.LogWarning("Post {PostId} already in favorites for user {UserId}.", postId, userId);
                return Result<bool>.Failure("Оголошення вже в улюблених.");
            }

            var favorite = new Favorites
            {
                UserId = userId,
                PostId = postId,
            };

            await this.favoriteRepository.AddAsync(favorite);
            await this.favoriteRepository.SaveChangesAsync();

            this.logger.LogInformation("Post {PostId} added to favorites for user {UserId}.", postId, userId);

            return true;
        }

        public async Task<Result<bool>> RemoveFromFavoritesAsync(int userId, int postId)
        {
            this.logger.LogInformation("User {UserId} removing post {PostId} from favorites.", userId, postId);

            var favorite = await this.favoriteRepository.GetAsync(userId, postId);

            if (favorite == null)
            {
                this.logger.LogWarning("Post {PostId} not in favorites for user {UserId}.", postId, userId);
                return Result<bool>.Failure("Оголошення не знайдено в улюблених.");
            }

            await this.favoriteRepository.RemoveAsync(favorite);
            await this.favoriteRepository.SaveChangesAsync();

            this.logger.LogInformation("Post {PostId} removed from favorites for user {UserId}.", postId, userId);

            return true;
        }

        public async Task<Result<HashSet<int>>> GetFavoritePostIdsAsync(int userId)
        {
            this.logger.LogInformation("Fetching favorite post IDs for user {UserId}.", userId);

            var ids = await this.favoriteRepository.GetFavoritePostIdsAsync(userId);

            return ids;
        }
    }
}