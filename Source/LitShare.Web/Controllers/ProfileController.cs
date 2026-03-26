namespace LitShare.Web.Controllers
{
    using System.Security.Claims;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService profileService;
        private readonly ILogger<ProfileController> logger;

        public ProfileController(
            IProfileService profileService,
            ILogger<ProfileController> logger)
        {
            this.profileService = profileService;
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
                return View(model);
            }

            var dto = new UpdateProfileDto
            {
                Email = model.Email,
                Region = model.Region,
                Phone = model.Phone,
                District = model.District,
                City = model.City,
                About = model.About
            };

            var result = await this.profileService.UpdateProfileAsync(userId, dto);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Profile update failed. UserId: {UserId}, Error: {Error}", userId, result.Error);

                ModelState.AddModelError(string.Empty, result.Error);
                return View(model);
            }

            this.logger.LogInformation("Profile updated successfully. UserId: {UserId}", userId);
            return RedirectToAction("MyProfile");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAvatar()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("User clicked generate avatar. UserId: {UserId}", userId);

            var result = await this.profileService.GenerateRandomAvatarAsync(userId);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Avatar generation failed. UserId: {UserId}", userId);
                return this.Content(result.Error);
            }

            return RedirectToAction("EditProfile");
        }

        public IActionResult DeleteAccount()
        {
            this.logger.LogWarning("User opened DeleteAccount page");
            return this.Content("Функція видалення акаунту ще не реалізована");
        }

        private int GetCurrentUserId()
        {
            var userIdString = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString!);
        }
    }
}