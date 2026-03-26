namespace LitShare.Web.Controllers
{
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;
        private readonly ILogger<HomeController> logger;

        public HomeController(IHomeService homeService, ILogger<HomeController> logger)
        {
            this.homeService = homeService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            this.logger.LogInformation("User navigated to home page.");

            ViewData["ShowSearch"] = true;

            var result = await this.homeService.GetAllPostsAsync();

            if (result.IsFailure)
            {
                this.logger.LogWarning("Failed to load posts: {Error}", result.Error);
                return this.View(new HomeViewModel());
            }

            var model = new HomeViewModel
            {
                Posts = result.Value!,
            };

            return this.View(model);
        }

        [HttpGet]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}