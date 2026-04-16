namespace LitShare.BLL.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class EditPostService : IEditPostService
    {
        private readonly IPostRepository postRepository;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<EditPostService> logger;
        private readonly AppSettings settings;

        public EditPostService(
            IPostRepository postRepository,
            IWebHostEnvironment environment,
            ILogger<EditPostService> logger,
            IOptions<AppSettings> options)
        {
            this.postRepository = postRepository;
            this.environment = environment;
            this.logger = logger;
            this.settings = options.Value;
        }

        public async Task<Result<PostViewDto>> GetPostByIdAsync(int id, int currentUserId)
        {
            var post = await this.postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return Result<PostViewDto>.Failure("Оголошення не знайдено.");
            }

            if (post.UserId != currentUserId)
            {
                return Result<PostViewDto>.Unauthorized("Доступ заборонено.");
            }

            var dto = new PostViewDto
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Author = post.Author ?? string.Empty,
                Description = post.Description,
                DealType = post.DealType,
                PhotoUrl = post.PhotoUrl,
                GenreIds = post.BookGenres.Select(bg => bg.GenreId).ToList(),
            };

            return dto;
        }

        public async Task<Result<bool>> EditPostAsync(EditPostDto dto, int currentUserId)
        {
            this.logger.LogInformation("Starting post edit for post ID: {PostId} by User: {UserId}", dto.PostId, currentUserId);

            var post = await this.postRepository.GetByIdAsync(dto.PostId);

            if (post == null)
            {
                return Result<bool>.Failure("Оголошення не знайдено.");
            }

            if (post.UserId != currentUserId)
            {
                return Result<bool>.Unauthorized("У вас немає прав для редагування цього оголошення.");
            }

            post.Title = dto.Title;
            post.Author = dto.Author;
            post.Description = dto.Description;
            post.DealType = (DealType)dto.DealTypeId;

            if (dto.NewPhoto != null && dto.NewPhoto.Length > 0)
            {
                if (dto.NewPhoto.Length > this.settings.MaxImageSizeBytes)
                {
                    this.logger.LogWarning(
                        "Upload failed: Image too large ({Size} bytes). Max allowed: {Max} bytes.",
                        dto.NewPhoto.Length,
                        this.settings.MaxImageSizeBytes);

                    double maxSizeMb = this.settings.MaxImageSizeBytes / 1048576.0;
                    return Result<bool>.Failure($"Розмір зображення перевищує максимально допустимий ({maxSizeMb:F1} МБ).");
                }

                string? newPhotoUrl = await this.SaveImageAsync(dto);
                if (newPhotoUrl != null)
                {
                    post.PhotoUrl = newPhotoUrl;
                }
            }

            post.BookGenres.Clear();
            foreach (var genreId in dto.GenreIds)
            {
                post.BookGenres.Add(new BookGenres { GenreId = genreId });
            }

            await this.postRepository.UpdateAsync(post);
            await this.postRepository.SaveChangesAsync();

            this.logger.LogInformation("Successfully edited post with ID: {PostId}", dto.PostId);
            return true;
        }

        private async Task<string?> SaveImageAsync(EditPostDto dto)
        {
            if (dto.NewPhoto == null || dto.NewPhoto.Length == 0)
            {
                return null;
            }

            string uploadsFolder = Path.Combine(this.environment.WebRootPath, "images", "posts");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string savedFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.NewPhoto.FileName);
            string filePath = Path.Combine(uploadsFolder, savedFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.NewPhoto.CopyToAsync(fileStream);
            }

            return "/images/posts/" + savedFileName;
        }
    }
}