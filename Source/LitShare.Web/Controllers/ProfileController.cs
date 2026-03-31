namespace LitShare.Web.Controllers
{
    using System.Security.Claims;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService profileService;
        private readonly IPostService postService;
        private readonly ILogger<ProfileController> logger;

        public ProfileController(
            IProfileService profileService,
            IPostService postService,
            ILogger<ProfileController> logger)
        {
            this.profileService = profileService;
            this.postService = postService;
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
                return this.Content(result.Error);
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
                PhotoUrl = user.PhotoUrl,
                About = user.About ?? string.Empty
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

            var booksResult = await this.postService.GetPostsByUserIdAsync(id);

            var books = booksResult.IsSuccess ? booksResult.Value! : new List<PostCardDto>();

            var model = new ProfileViewModel
            {
                Name = user.Name ?? "Користувач",
                Email = user.Email ?? string.Empty,
                Region = user.Region ?? "Не вказано",
                District = user.District ?? "Не вказано",
                City = user.City ?? "Не вказано",
                PhotoUrl = user.PhotoUrl,
                Phone = user.Phone ?? "Номер не вказано",
                About = user.About ?? "Інформація відсутня",
                UserBooks = books
            };

            return this.View(model);
        }

        public IActionResult MyBooks()
        {
            this.logger.LogInformation("User opened MyBooks page");
            return this.Content("Сторінка 'Мої книги' ще не готова");
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("Opening EditProfile page. UserId: {UserId}", userId);

            var result = await this.profileService.GetUserByIdAsync(userId);

            if (result.IsFailure)
            {
                return this.Content(result.Error);
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
                PhotoUrl = user.PhotoUrl
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
                PhotoUrl = model.PhotoUrl
            };

            var result = await this.profileService.UpdateProfileAsync(userId, dto);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Profile update failed. UserId: {UserId}, Error: {Error}", userId, result.Error);
                this.ModelState.AddModelError(string.Empty, result.Error);
                return this.View(model);
            }

            this.logger.LogInformation("Profile updated successfully. UserId: {UserId}", userId);

            return this.RedirectToAction("MyProfile");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAvatar()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("User clicked generate avatar. UserId: {UserId}", userId);

            var result = await this.profileService.GetUserByIdAsync(userId);

            if (result.IsFailure)
            {
                return this.Content(result.Error);
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
                UserBooks = new List<PostCardDto>()
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
                return this.Content(result.Error);
            }

            await this.HttpContext.SignOutAsync();

            return this.RedirectToAction("Index", "Home");
}

        private int GetCurrentUserId()
        {
            var userIdString = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString!);
        }
    }
}