namespace LitShare.Web.Controllers
{
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize(Roles = "User, Admin")]
    public class ProfileController : BaseController
    {
        private readonly IProfileService profileService;
        private readonly IPostService postService;
        private readonly IReviewService reviewService;
        private readonly IFavoriteService favoriteService;
        private readonly ILogger<ProfileController> logger;

        public ProfileController(
            IProfileService profileService,
            IPostService postService,
            IReviewService reviewService,
            IFavoriteService favoriteService,
            ILogger<ProfileController> logger)
        {
            this.profileService = profileService;
            this.postService = postService;
            this.reviewService = reviewService;
            this.favoriteService = favoriteService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("Opening profile page. UserId: {UserId}", userId);

            var result = await this.profileService.GetUserByIdAsync(userId);

            if (result.IsFailure)
            {
                this.logger.LogWarning("User not found. UserId: {UserId}", userId);
                return this.HandleFailure(result.Error);
            }

            var user = result.Value!;

            var model = new ProfileViewModel
            {
                UserId = userId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone ?? string.Empty,
                Region = user.Region ?? string.Empty,
                District = user.District ?? string.Empty,
                City = user.City ?? string.Empty,
                PhotoUrl = user.PhotoUrl,
                About = user.About ?? string.Empty,
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GuestProfile(int id)
        {
            this.logger.LogInformation("Request to view guest profile. Target UserId: {UserId}", id);

            var result = await this.profileService.GetUserByIdAsync(id);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Guest profile not found. UserId: {UserId}. Reason: {Error}", id, result.Error);
                return this.NotFound("Користувача не знайдено");
            }

            var user = result.Value!;

            var booksResult = await this.postService.GetPostsByUserIdAsync(id, true);
            var books = booksResult.IsSuccess ? booksResult.Value! : new List<PostCardDto>();

            var reviewsResult = await this.reviewService.GetReviewsByUserIdAsync(id);
            var reviewCount = reviewsResult.IsSuccess ? reviewsResult.Value!.Count() : 0;

            var model = new ProfileViewModel
            {
                UserId = id,
                Name = user.Name ?? "Користувач",
                Email = user.Email ?? string.Empty,
                Region = user.Region ?? "Не вказано",
                District = user.District ?? "Не вказано",
                City = user.City ?? "Не вказано",
                PhotoUrl = user.PhotoUrl,
                Phone = user.Phone ?? "Номер не вказано",
                About = user.About ?? "Інформація відсутня",
                UserBooks = books,
                ReviewCount = reviewCount,
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("Opening EditProfile page. UserId: {UserId}", userId);

            var result = await this.profileService.GetUserByIdAsync(userId);

            if (result.IsFailure)
            {
                return this.HandleFailure(result.Error);
            }

            var user = result.Value!;

            var model = new ProfileViewModel
            {
                Name = user.Name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.Phone ?? string.Empty,
                Region = user.Region ?? string.Empty,
                District = user.District ?? string.Empty,
                City = user.City ?? string.Empty,
                About = user.About ?? string.Empty,
                PhotoUrl = user.PhotoUrl,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("User updating profile. UserId: {UserId}", userId);

            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var dto = new UpdateProfileDto
            {
                Email = model.Email,
                Region = model.Region,
                Phone = model.Phone,
                District = model.District,
                City = model.City,
                About = model.About,
                PhotoUrl = model.PhotoUrl,
            };

            var result = await this.profileService.UpdateProfileAsync(userId, dto);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Profile update failed. UserId: {UserId}, Error: {Error}", userId, result.Error);
                this.ModelState.AddModelError(string.Empty, result.Error);
                return this.View(model);
            }

            this.logger.LogInformation("Profile updated successfully. UserId: {UserId}", userId);

            if (User.IsInRole("Admin"))
            {
                return this.RedirectToAction("MyProfile", "Admin");
            }

            return this.RedirectToAction("MyProfile", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAvatar()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("User clicked generate avatar. UserId: {UserId}", userId);

            var result = await this.profileService.GetUserByIdAsync(userId);

            if (result.IsFailure)
            {
                return this.HandleFailure(result.Error);
            }

            var user = result.Value!;

            var seed = Guid.NewGuid().ToString();
            var tempAvatar = $"https://api.dicebear.com/7.x/bottts/svg?seed={seed}";

            var model = new ProfileViewModel
            {
                Name = user.Name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.Phone ?? string.Empty,
                Region = user.Region ?? string.Empty,
                District = user.District ?? string.Empty,
                City = user.City ?? string.Empty,
                About = user.About ?? string.Empty,
                PhotoUrl = tempAvatar,
                UserBooks = new List<PostCardDto>(),
            };

            return this.View("EditProfile", model);
        }

        [HttpGet]
        public IActionResult DeleteAccount()
        {
            this.logger.LogInformation("User opened DeleteAccount page");
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountConfirmed()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("User confirmed account deletion. UserId: {UserId}", userId);

            var result = await this.profileService.DeleteAccountAsync(userId);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Delete failed. UserId: {UserId}, Error: {Error}", userId, result.Error);
                return this.HandleFailure(result.Error);
            }

            await this.HttpContext.SignOutAsync();

            return this.RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> MyBooks(bool? isActive)
        {
            var userId = this.GetCurrentUserId();
            this.logger.LogInformation("User opened MyBooks page. UserId: {UserId}, Filter: {IsActive}", userId, isActive);

            var booksResult = await this.postService.GetPostsByUserIdAsync(userId, isActive);
            var books = booksResult.IsSuccess ? booksResult.Value! : new List<PostCardDto>();

            var favResult = await this.favoriteService.GetFavoritePostIdsAsync(userId);
            var favoriteIds = favResult.IsSuccess ? favResult.Value! : new HashSet<int>();

            var model = new ProfileViewModel
            {
                UserBooks = books,
                FavoritePostIds = favoriteIds,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostStatus(int postId)
        {
            var userId = this.GetCurrentUserId();

            var postsResult = await this.postService.GetPostsByUserIdAsync(userId, null);

            if (postsResult.IsFailure)
            {
                return this.HandleFailure(postsResult.Error);
            }

            var post = postsResult.Value!.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return this.HandleFailure("Пост не знайдено");
            }

            var result = await this.postService.SetPostStatusAsync(postId, userId, !post.IsActive);

            if (result.IsFailure)
            {
                return this.HandleFailure(result.Error);
            }

            return this.RedirectToAction("MyBooks");
        }

        [HttpGet]
        public IActionResult DeletePost(int postId)
        {
            this.logger.LogInformation("User opened delete post confirmation. PostId: {PostId}", postId);
            ViewBag.PostId = postId;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePostConfirmed(int postId)
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("User confirmed post deletion. UserId: {UserId}, PostId: {PostId}", userId, postId);

            var result = await this.postService.DeletePostAsync(postId, userId);

            if (result.IsFailure)
            {
                return this.HandleFailure(result.Error);
            }

            return this.RedirectToAction("MyBooks");
        }
    }
}