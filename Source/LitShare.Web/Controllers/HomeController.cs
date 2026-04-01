namespace LitShare.Web.Controllers
{
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    // [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;
        private readonly IGenreService genreService;
        private readonly ILogger<HomeController> logger;

        public HomeController(IHomeService homeService, IGenreService genreService, ILogger<HomeController> logger)
        {
            this.homeService = homeService;
            this.genreService = genreService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm,
            string? location,
            DealType? dealType,
            List<int>? genres)
        {
            this.logger.LogInformation("User navigated to home page with filters.");
            this.logger.LogInformation(
                "SearchTerm: {SearchTerm}, Location: {Location}, DealType: {DealType}, Genres: {GenresCount}",
                searchTerm,
                location,
                dealType,
                genres?.Count ?? 0);

            ViewData["ShowSearch"] = true;

            var genresResult = await this.genreService.GetAllGenresAsync();

            var filter = new PostFilterDto
            {
                SearchTerm = searchTerm,
                Location = location,
                DealType = dealType,
                GenreIds = genres,
            };

            var result = await this.homeService.GetFilteredPostsAsync(filter);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Failed to load posts: {Error}", result.Error);
                return this.View(new HomeViewModel
                {
                    AllGenres = genresResult.IsSuccess ? genresResult.Value! : new List<GenreDto>(),
                });
            }

            var model = new HomeViewModel
            {
                Posts = result.Value!,
                SearchTerm = searchTerm,
                Location = location,
                DealType = dealType,
                SelectedGenres = genres,
                AllGenres = genresResult.IsSuccess ? genresResult.Value! : new List<GenreDto>(),
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