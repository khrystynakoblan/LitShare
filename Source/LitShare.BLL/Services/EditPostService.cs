namespace LitShare.BLL.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    public class EditPostService : IEditPostService
    {
        private readonly IPostRepository postRepository;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<EditPostService> logger;

        public EditPostService(
            IPostRepository postRepository,
            IWebHostEnvironment environment,
            ILogger<EditPostService> logger)
        {
            this.postRepository = postRepository;
            this.environment = environment;
            this.logger = logger;
        }

        public async Task<PostViewDto?> GetPostByIdAsync(int id)
        {
            this.logger.LogInformation("Fetching post with ID: {Id}", id);
            var post = await this.postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return null;
            }

            return new PostViewDto
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Author = post.Author ?? string.Empty,
                Description = post.Description,
                DealType = post.DealType,
                PhotoUrl = post.PhotoUrl,
                GenreId = post.BookGenres.FirstOrDefault()?.GenreId ?? 0,
            };
        }

        public async Task EditPostAsync(EditPostDto dto)
        {
            this.logger.LogInformation("Starting post edit for post ID: {PostId}", dto.PostId);
            try
            {
                var post = await this.postRepository.GetByIdAsync(dto.PostId);
                if (post == null)
                {
                    this.logger.LogWarning("Post with ID: {PostId} was not found", dto.PostId);
                    throw new InvalidOperationException($"Post with ID {dto.PostId} not found");
                }

                post.Title = dto.Title;
                post.Author = dto.Author;
                post.Description = dto.Description;
                post.DealType = (DealType)dto.DealTypeId;
                post.BookGenres.Clear();
                post.BookGenres.Add(new BookGenres { GenreId = dto.GenreId });

                var newPhotoUrl = await this.SaveImageAsync(dto);
                if (newPhotoUrl != null)
                {
                    post.PhotoUrl = newPhotoUrl;
                }

                await this.postRepository.UpdateAsync(post);
                await this.postRepository.SaveChangesAsync();
                this.logger.LogInformation("Successfully edited post with ID: {PostId}", dto.PostId);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                this.logger.LogError(ex, "Failed to edit post with ID: {PostId}. Error: {Message}", dto.PostId, ex.Message);
                throw;
            }
        }

        private async Task<string?> SaveImageAsync(EditPostDto dto)
        {
            if (dto.NewPhoto == null || dto.NewPhoto.Length == 0)
            {
                return null;
            }

            this.logger.LogInformation("Processing image upload: {FileName}", dto.NewPhoto.FileName);

            string uploadsFolder = Path.Combine(this.environment.WebRootPath, "images", "posts");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                this.logger.LogInformation("Created directory for post images: {Path}", uploadsFolder);
            }

            string savedFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.NewPhoto.FileName);
            string filePath = Path.Combine(uploadsFolder, savedFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.NewPhoto.CopyToAsync(fileStream);
            }

            this.logger.LogInformation("Image saved as: {SavedName}", savedFileName);
            return "/images/posts/" + savedFileName;
        }
    }
}