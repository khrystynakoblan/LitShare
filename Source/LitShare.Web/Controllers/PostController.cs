namespace LitShare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;

    public class PostController : Controller
    {
        private readonly ICreatePostService createPostService;
        private readonly IGenreRepository genreRepository;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<PostController> logger;

        public PostController(
            ICreatePostService createPostService,
            IGenreRepository genreRepository,
            IWebHostEnvironment environment,
            ILogger<PostController> logger)
        {
            this.createPostService = createPostService;
            this.genreRepository = genreRepository;
            this.environment = environment;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePost()
        {
            this.logger.LogInformation("Navigating to CreatePost page.");
            var model = new CreatePostModel();
            await this.PopulateSelectionLists(model);
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostModel model)
        {
            this.logger.LogInformation("Received request to create a new post with title: {Title}", model.Title);

            if (!this.ModelState.IsValid)
            {
                this.logger.LogWarning("Model state is invalid for post creation.");
                await this.PopulateSelectionLists(model);
                return this.View(model);
            }

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
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while saving the post to the database.");
                this.ModelState.AddModelError(string.Empty, "Сталася помилка при збереженні в базу даних.");
                await this.PopulateSelectionLists(model);
                return this.View(model);
            }
        }

        private async Task PopulateSelectionLists(CreatePostModel model)
        {
            this.logger.LogInformation("Populating selection lists from database.");
            var allGenres = await this.genreRepository.GetAllAsync();

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