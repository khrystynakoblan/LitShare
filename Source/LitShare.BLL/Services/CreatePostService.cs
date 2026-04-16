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

    public class CreatePostService : ICreatePostService
    {
        private readonly IPostRepository postRepository;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<CreatePostService> logger;
        private readonly AppSettings settings;

        public CreatePostService(
            IPostRepository postRepository,
            IWebHostEnvironment environment,
            ILogger<CreatePostService> logger,
            IOptions<AppSettings> options)
        {
            this.postRepository = postRepository;
            this.environment = environment;
            this.logger = logger;
            this.settings = options.Value;
        }

        public async Task<Result<int>> CreatePostAsync(CreatePostDto dto, int userId)
        {
            this.logger.LogInformation("Starting post creation for user ID: {UserId}. Title: {Title}", userId, dto.Title);

            if (dto.ImageFile != null && dto.ImageFile.Length > this.settings.MaxImageSizeBytes)
            {
                this.logger.LogWarning(
                    "Upload failed: Image too large ({Size} bytes). Max allowed: {Max} bytes.",
                    dto.ImageFile.Length,
                    this.settings.MaxImageSizeBytes);

                double maxSizeMb = this.settings.MaxImageSizeBytes / 1048576.0;

                return Result<int>.Failure($"Розмір зображення перевищує максимально допустимий ({maxSizeMb:F1} МБ).");
            }

            string? photoUrl = await this.SaveImageAsync(dto);

            var newPost = new Posts
            {
                UserId = userId,
                Title = dto.Title,
                Author = dto.Author,
                DealType = (DealType)dto.DealTypeId,
                Description = dto.Description,
                PhotoUrl = photoUrl,
                BookGenres = dto.GenreIds.Select(genreId => new BookGenres { GenreId = genreId }).ToList(),
            };

            await this.postRepository.AddAsync(newPost);
            await this.postRepository.SaveChangesAsync();

            this.logger.LogInformation("Successfully created post with ID: {PostId} for user ID: {UserId}", newPost.Id, userId);

            return newPost.Id;
        }

        private async Task<string?> SaveImageAsync(CreatePostDto dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
            {
                return null;
            }

            this.logger.LogInformation("Processing image upload: {FileName}", dto.ImageFile.FileName);

            string uploadsFolder = Path.Combine(this.environment.WebRootPath, "images", "posts");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                this.logger.LogInformation("Created directory for post images: {Path}", uploadsFolder);
            }

            string savedFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, savedFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(fileStream);
            }

            this.logger.LogInformation("Image saved as: {SavedName}", savedFileName);
            return "/images/posts/" + savedFileName;
        }
    }
}