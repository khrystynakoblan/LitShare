namespace LitShare.Web.Controllers
{
    using System;
<<<<<<< HEAD
=======
    using System.Collections.Generic;
    using System.IO;
>>>>>>> ef4b67b (Add Create Post Window and tests)
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
<<<<<<< HEAD
    using LitShare.Web.Models;
=======
    using LitShare.DAL.Repositories.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Hosting;
>>>>>>> ef4b67b (Add Create Post Window and tests)
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;

    public class PostController : Controller
    {
        private readonly ICreatePostService createPostService;
<<<<<<< HEAD
        private readonly IGenreService genreService;
=======
        private readonly IGenreRepository genreRepository;
        private readonly IWebHostEnvironment environment;
>>>>>>> ef4b67b (Add Create Post Window and tests)
        private readonly ILogger<PostController> logger;

        public PostController(
            ICreatePostService createPostService,
<<<<<<< HEAD
            IGenreService genreService,
            ILogger<PostController> logger)
        {
            this.createPostService = createPostService;
            this.genreService = genreService;
=======
            IGenreRepository genreRepository,
            IWebHostEnvironment environment,
            ILogger<PostController> logger)
        {
            this.createPostService = createPostService;
            this.genreRepository = genreRepository;
            this.environment = environment;
>>>>>>> ef4b67b (Add Create Post Window and tests)
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePost()
        {
<<<<<<< HEAD
            this.logger.LogInformation("User navigated to the Create Post page.");

            var model = new CreatePostModel();
            await this.PopulateSelectionLists(model);

=======
            this.logger.LogInformation("Navigating to CreatePost page.");
            var model = new CreatePostModel();
            await this.PopulateSelectionLists(model);
>>>>>>> ef4b67b (Add Create Post Window and tests)
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostModel model)
        {
<<<<<<< HEAD
            this.logger.LogInformation("Processing post creation request for title: {Title}", model.Title);

            if (!this.ModelState.IsValid)
            {
                this.logger.LogWarning("Model state is invalid. Returning to the creation view.");
=======
            this.logger.LogInformation("Received request to create a new post with title: {Title}", model.Title);

            if (!this.ModelState.IsValid)
            {
                this.logger.LogWarning("Model state is invalid for post creation.");
>>>>>>> ef4b67b (Add Create Post Window and tests)
                await this.PopulateSelectionLists(model);
                return this.View(model);
            }

<<<<<<< HEAD
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
=======
            string? savedFileName = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                this.logger.LogInformation("Processing image upload: {FileName}", model.ImageFile.FileName);
                string uploadsFolder = Path.Combine(this.environment.WebRootPath, "images", "posts");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    this.logger.LogInformation("Created directory for post images: {Path}", uploadsFolder);
                }

                savedFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, savedFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                this.logger.LogInformation("Image saved as: {SavedName}", savedFileName);
            }

            var dto = new CreatePostDto
            {
                Title = model.Title,
                Author = model.Author,
                GenreId = model.GenreId,
                DealTypeId = model.DealTypeId,
                Description = model.Description,
                PhotoUrl = savedFileName != null ? "/images/posts/" + savedFileName : null
            };

            try
            {
                int currentUserId = 1;
                this.logger.LogInformation("Calling service to save post for user: {UserId}", currentUserId);

                await this.createPostService.CreatePostAsync(dto, currentUserId);

                this.logger.LogInformation("Post created successfully. Redirecting to Home.");
>>>>>>> ef4b67b (Add Create Post Window and tests)
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                this.logger.LogError(ex, "An unexpected error occurred during post creation.");

                this.ModelState.AddModelError(string.Empty, "Сталася помилка при збереженні. Спробуйте пізніше.");
                await this.PopulateSelectionLists(model);

=======
                this.logger.LogError(ex, "An error occurred while saving the post to the database.");
                this.ModelState.AddModelError(string.Empty, "Сталася помилка при збереженні в базу даних.");
                await this.PopulateSelectionLists(model);
>>>>>>> ef4b67b (Add Create Post Window and tests)
                return this.View(model);
            }
        }

        private async Task PopulateSelectionLists(CreatePostModel model)
        {
<<<<<<< HEAD
            this.logger.LogInformation("Fetching genres and deal types for dropdowns.");

            var allGenres = await this.genreService.GetAllGenresAsync();
=======
            this.logger.LogInformation("Populating selection lists from database.");
            var allGenres = await this.genreRepository.GetAllAsync();
>>>>>>> ef4b67b (Add Create Post Window and tests)

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