namespace LitShare.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class ReviewController : Controller
    {
        private readonly IReviewService reviewService;
        private readonly IProfileService profileService;
        private readonly ILogger<ReviewController> logger;

        public ReviewController(
            IReviewService reviewService,
            IProfileService profileService,
            ILogger<ReviewController> logger)
        {
            this.reviewService = reviewService;
            this.profileService = profileService;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int userId)
        {
            this.logger.LogInformation("Navigating to reviews page for user ID: {UserId}", userId);

            var userResult = await this.profileService.GetUserByIdAsync(userId);

            if (userResult.IsFailure)
            {
                return this.NotFound();
            }

            var reviewsResult = await this.reviewService.GetReviewsByUserIdAsync(userId);
            var reviews = reviewsResult.IsSuccess ? reviewsResult.Value! : Enumerable.Empty<ReviewDto>();
            var reviewList = reviews.ToList();

            bool canLeaveReview = false;

            if (this.User.Identity != null && this.User.Identity.IsAuthenticated)
            {
                int currentUserId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier) !);
                var hasReviewed = await this.reviewService.HasReviewedAsync(currentUserId, userId);
                canLeaveReview = !hasReviewed.Value && currentUserId != userId;
            }

            var model = new ReviewListViewModel
            {
                ReviewedUserId = userId,
                ReviewedUserName = userResult.Value!.Name ?? "Користувач",
                AverageRating = reviewList.Any() ? reviewList.Average(r => r.Rating) : 0,
                TotalReviews = reviewList.Count,
                CanLeaveReview = canLeaveReview,
                Reviews = reviewList,
            };

            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int userId)
        {
            this.logger.LogInformation("Navigating to create review page for user ID: {UserId}", userId);

            var userResult = await this.profileService.GetUserByIdAsync(userId);

            if (userResult.IsFailure)
            {
                return this.NotFound();
            }

            int currentUserId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier) !);

            var hasReviewed = await this.reviewService.HasReviewedAsync(currentUserId, userId);

            if (hasReviewed.Value)
            {
                return this.RedirectToAction("Index", new { userId });
            }

            var model = new ReviewViewModel
            {
                ReviewedUserId = userId,
                ReviewedUserName = userResult.Value!.Name ?? "Користувач",
            };

            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            int currentUserId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier) !);

            var dto = new CreateReviewDto
            {
                ReviewedUserId = model.ReviewedUserId,
                Rating = model.Rating,
                Text = model.Text,
            };

            var result = await this.reviewService.CreateReviewAsync(dto, currentUserId);

            if (result.IsFailure)
            {
                this.ModelState.AddModelError(string.Empty, result.Error);
                return this.View(model);
            }

            return this.RedirectToAction("Index", new { userId = model.ReviewedUserId, success = true });
        }
    }
}