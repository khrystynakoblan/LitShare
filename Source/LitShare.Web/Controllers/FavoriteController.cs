namespace LitShare.Web.Controllers
{
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize(Roles = "User")]
    public class FavoriteController : BaseController
    {
        private readonly IFavoriteService favoriteService;
        private readonly ILogger<FavoriteController> logger;

        public FavoriteController(IFavoriteService favoriteService, ILogger<FavoriteController> logger)
        {
            this.favoriteService = favoriteService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = this.GetCurrentUserId();

            this.logger.LogInformation("User {UserId} opened favorites page.", userId);

            var result = await this.favoriteService.GetFavoritesAsync(userId);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Failed to load favorites for user {UserId}: {Error}", userId, result.Error);
                return this.View(new FavoriteViewModel());
            }

            var model = new FavoriteViewModel
            {
                Favorites = result.Value!,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int postId, string returnUrl = "/")
        {
            int userId = this.GetCurrentUserId();

            this.logger.LogInformation("User {UserId} toggling favorite for post {PostId}.", userId, postId);

            bool isFavorited = (await this.favoriteService.GetFavoritePostIdsAsync(userId))
                .Value?.Contains(postId) ?? false;

            if (isFavorited)
            {
                await this.favoriteService.RemoveFromFavoritesAsync(userId, postId);
                this.logger.LogInformation("Post {PostId} removed from favorites.", postId);
            }
            else
            {
                await this.favoriteService.AddToFavoritesAsync(userId, postId);
                this.logger.LogInformation("Post {PostId} added to favorites.", postId);
            }

            return this.Redirect(returnUrl);
        }
    }
}