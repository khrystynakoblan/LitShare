namespace LitShare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;

    public class PostController : Controller
    {
        private readonly ICreatePostService createPostService;
        private readonly IEditPostService editPostService;
        private readonly IGenreService genreService;
        private readonly IPostRepository postRepository;
        private readonly ILogger<PostController> logger;

        public PostController(
            ICreatePostService createPostService,
            IEditPostService editPostService,
            IGenreService genreService,
            IPostRepository postRepository,
            ILogger<PostController> logger)
        {
            this.createPostService = createPostService;
            this.editPostService = editPostService;
            this.genreService = genreService;
            this.postRepository = postRepository;
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

            try
            {
                int currentUserId = 1;
                this.logger.LogInformation("Calling service to save post for user: {UserId}", currentUserId);

                var dto = new CreatePostDto
                {
                    Title = model.Title,
                    Author = model.Author,
                    GenreIds = model.SelectedGenreIds,
                    DealTypeId = model.DealTypeId,
                    Description = model.Description,
                    ImageFile = model.ImageFile,
                };

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await this.editPostService.GetPostByIdAsync(id);
            if (post == null)
            {
                return this.NotFound();
            }

            var model = new PostEditViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Author = post.Author,
                Description = post.Description,
                DealType = post.DealType,
                PhotoUrl = post.PhotoUrl,
                SelectedGenreIds = post.GenreIds,
                Genres = await this.BuildGenreMultiSelectListAsync(post.GenreIds),
                DealTypes = this.BuildDealTypeSelectList((int)post.DealType),
            };

            return this.View("~/Views/Post/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostEditViewModel model)
        {
            if (id != model.Id)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                model.Genres = await this.BuildGenreMultiSelectListAsync(model.SelectedGenreIds);
                model.DealTypes = this.BuildDealTypeSelectList((int)model.DealType);
                return this.View("~/Views/Post/Edit.cshtml", model);
            }

            try
            {
                var dto = new EditPostDto
                {
                    PostId = id,
                    Title = model.Title,
                    Author = model.Author,
                    Description = model.Description,
                    DealTypeId = (int)model.DealType,
                    GenreIds = model.SelectedGenreIds,
                };

                await this.editPostService.EditPostAsync(dto);

                this.logger.LogInformation("Post with id {Id} updated successfully.", id);
                return this.RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException)
            {
                return this.NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while editing the post.");
                this.ModelState.AddModelError(string.Empty, "Сталася помилка при збереженні в базу даних.");
                model.Genres = await this.BuildGenreMultiSelectListAsync(model.SelectedGenreIds);
                model.DealTypes = this.BuildDealTypeSelectList((int)model.DealType);
                return this.View("~/Views/Post/Edit.cshtml", model);
            }
        }

        private async Task<SelectList> BuildGenreSelectListAsync(int selectedGenreId = 0)
        {
            var genres = await this.genreService.GetAllGenresAsync();
            return new SelectList(genres, "Id", "Name", selectedGenreId);
        }

        private SelectList BuildDealTypeSelectList(int? selectedValue = null)
        {
            var items = Enum.GetValues(typeof(DealType))
                .Cast<DealType>()
                .Select(d => new { Value = (int)d, Text = d == DealType.Exchange ? "Обмін" : "Дарування" });
            return new SelectList(items, "Value", "Text", selectedValue);
        }

        private async Task PopulateSelectionLists(CreatePostModel model)
        {
            this.logger.LogInformation("Populating selection lists from database.");
            var genres = await this.genreService.GetAllGenresAsync();

            model.Genres = new MultiSelectList(genres, "Id", "Name", model.SelectedGenreIds);

            model.DealTypes = Enum.GetValues(typeof(DealType))
                .Cast<DealType>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d == DealType.Exchange ? "Обмін" : "Дарування",
                }).ToList();
        }

        private async Task<MultiSelectList> BuildGenreMultiSelectListAsync(List<int> selectedGenreIds)
        {
            var genres = await this.genreService.GetAllGenresAsync();
            return new MultiSelectList(genres, "Id", "Name", selectedGenreIds);
        }
    }
}