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
    using static System.Reflection.Metadata.BlobBuilder;

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

            this.logger.LogInformation("User found: {UserName}. Fetching books for UserId: {UserId}", user.Name, id);

            var booksResult = await this.postService.GetPostsByUserIdAsync(id);

            if (booksResult.IsFailure)
            {
                this.logger.LogError("Failed to fetch books for UserId: {UserId}. Error: {Error}", id, booksResult.Error);
            }

            var books = booksResult.IsSuccess ? booksResult.Value! : new List<PostCardDto>();

            var model = new ProfileViewModel
            {
                Name = user.Name ?? "Користувач",
                Email = user.Email ?? string.Empty,
                Region = user.Region ?? "Не вказано",
                District = user.District ?? "Не вказано",
                City = user.City ?? "Не вказано",
                PhotoUrl = user.PhotoUrl,
                About = user.About ?? "Інформація відсутня",
                UserBooks = books
            };

            this.logger.LogInformation("Successfully loaded guest profile for {UserName} (ID: {UserId}) with {BookCount} books.", user.Name, id, books.Count());

            return this.View(model);
        }

        public IActionResult MyBooks()
        {
            this.logger.LogInformation("User opened MyBooks page");
            return this.Content("Сторінка 'Мої книги' ще не готова");
        }

        public IActionResult EditProfile()
        {
            this.logger.LogInformation("User opened EditProfile page");
            return this.Content("Сторінка 'Редагувати профіль' ще не готова");
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