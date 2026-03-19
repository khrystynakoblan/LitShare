namespace LitShare.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;

    public class PostController : Controller
    {
        private readonly ICreatePostService createPostService;
        private readonly IGenreService genreService;
        private readonly ILogger<PostController> logger;

        public PostController(
            ICreatePostService createPostService,
            IGenreService genreService,
            ILogger<PostController> logger)
        {
            this.createPostService = createPostService;
            this.genreService = genreService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePost()
        {
            this.logger.LogInformation("User navigated to the Create Post page.");

            var model = new CreatePostModel();
            await this.PopulateSelectionLists(model);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostModel model)
        {
            this.logger.LogInformation("Processing post creation request for title: {Title}", model.Title);

            if (!this.ModelState.IsValid)
            {
                this.logger.LogWarning("Model state is invalid. Returning to the creation view.");
                await this.PopulateSelectionLists(model);
                return this.View(model);
            }

            try
            {
                var dto = new CreatePostDto
                {
                    Title = model.Title,
                    Author = model.Author,
                    GenreId = model.GenreId!.Value,
                    DealTypeId = model.DealTypeId!.Value,
                    Description = model.Description
                };

                int currentUserId = 1;

                this.logger.LogInformation("Calling CreatePostService for User ID: {UserId}", currentUserId);

                await this.createPostService.CreatePostAsync(dto, model.ImageFile, currentUserId);

                this.logger.LogInformation("Post created successfully. Redirecting to home.");
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An unexpected error occurred during post creation.");

                this.ModelState.AddModelError(string.Empty, "Сталася помилка при збереженні. Спробуйте пізніше.");
                await this.PopulateSelectionLists(model);

                return this.View(model);
            }
        }

        private async Task PopulateSelectionLists(CreatePostModel model)
        {
            this.logger.LogInformation("Fetching genres and deal types for dropdowns.");

            var allGenres = await this.genreService.GetAllGenresAsync();

            model.Genres = allGenres.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Name
            }).ToList();

            model.DealTypes = Enum.GetValues(typeof(DealType))
                .Cast<DealType>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d == DealType.Exchange ? "Обмін" : "Дарування"
                }).ToList();
        }
    }
}