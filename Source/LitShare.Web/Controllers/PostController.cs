namespace LitShare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;

    [Authorize(Roles = "User")]
    public class PostController : Controller
    {
        private readonly ICreatePostService createPostService;
        private readonly IEditPostService editPostService;
        private readonly IGenreService genreService;
        private readonly IPostService postService;
        private readonly ILogger<PostController> logger;

        public PostController(
            ICreatePostService createPostService,
            IEditPostService editPostService,
            IGenreService genreService,
            IPostService postService,
            ILogger<PostController> logger)
        {
            this.createPostService = createPostService;
            this.editPostService = editPostService;
            this.genreService = genreService;
            this.postService = postService;
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
            if (!this.ModelState.IsValid)
            {
                await this.PopulateSelectionLists(model);
                return this.View(model);
            }

            int currentUserId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier) !);

            var dto = new CreatePostDto
            {
                Title = model.Title,
                Author = model.Author,
                GenreIds = model.SelectedGenreIds,
                DealTypeId = model.DealTypeId,
                Description = model.Description,
                ImageFile = model.ImageFile,
            };

            var result = await this.createPostService.CreatePostAsync(dto, currentUserId);

            if (result.IsFailure)
            {
                this.ModelState.AddModelError(string.Empty, result.Error);
                await this.PopulateSelectionLists(model);
                return this.View(model);
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int currentUserId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier) !);
            var result = await this.editPostService.GetPostByIdAsync(id, currentUserId);

            if (result.IsFailure)
            {
                this.TempData["ErrorMessage"] = result.Error;
                return this.RedirectToAction("Index", "Home");
            }

            var post = result.Value!;
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

            int currentUserId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier) !);

            var dto = new EditPostDto
            {
                PostId = id,
                Title = model.Title,
                Author = model.Author,
                Description = model.Description,
                DealTypeId = (int)model.DealType,
                GenreIds = model.SelectedGenreIds,
                NewPhoto = model.NewPhoto
            };

            var result = await this.editPostService.EditPostAsync(dto, currentUserId);

            if (result.IsFailure)
            {
                if (result.IsUnauthorized)
                {
                    this.TempData["ErrorMessage"] = result.Error;
                    return this.RedirectToAction("Index", "Home");
                }

                this.ModelState.AddModelError(string.Empty, result.Error);
                model.Genres = await this.BuildGenreMultiSelectListAsync(model.SelectedGenreIds);
                model.DealTypes = this.BuildDealTypeSelectList((int)model.DealType);
                return this.View("~/Views/Post/Edit.cshtml", model);
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            this.logger.LogInformation("Navigating to Details page for post ID: {Id}", id);

            var result = await this.postService.GetPostDetailsAsync(id);

            if (result.IsFailure)
            {
                return this.NotFound();
            }

            var post = result.Value!;
            var model = new PostDetailsViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Author = post.Author,
                Description = post.Description,
                PhotoUrl = post.PhotoUrl,
                DealType = post.DealType,
                Genres = post.Genres,
                UserId = post.UserId,
            };

            return this.View(model);
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
            var genreResult = await this.genreService.GetAllGenresAsync();
            var genres = genreResult.IsSuccess ? genreResult.Value! : new List<GenreDto>();

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
            var genreResult = await this.genreService.GetAllGenresAsync();
            var genres = genreResult.IsSuccess ? genreResult.Value! : new List<GenreDto>();

            return new MultiSelectList(genres, "Id", "Name", selectedGenreIds);
        }
    }
}