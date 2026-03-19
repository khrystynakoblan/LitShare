namespace LitShare.Web.Controllers
{
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

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

            var user = await this.profileService.GetUserByIdAsync(userId);

            if (user == null)
            {
                this.logger.LogWarning("User not found. UserId: {UserId}", userId);
                return this.Content("User not found");
            }

            this.logger.LogInformation("User profile loaded. UserId: {UserId}", userId);

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

        public IActionResult AddBook()
        {
            this.logger.LogInformation("User opened AddBook page");
            return this.Content("Сторінка 'Додати книгу' ще не готова");
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
            return 1;
        }
    }
}