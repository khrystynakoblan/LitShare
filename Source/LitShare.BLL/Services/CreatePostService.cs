namespace LitShare.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class CreatePostService : ICreatePostService
    {
        private readonly IPostRepository postRepository;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<CreatePostService> logger;

        public CreatePostService(
            IPostRepository postRepository,
            IWebHostEnvironment environment,
            ILogger<CreatePostService> logger)
        {
            this.postRepository = postRepository;
            this.environment = environment;
            this.logger = logger;
        }

        public async Task<int> CreatePostAsync(CreatePostDto dto, IFormFile? imageFile, int userId)
        {
            this.logger.LogInformation("Starting post creation for user ID: {UserId}. Title: {Title}", userId, dto.Title);

            try
            {
                string? savedPhotoUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    savedPhotoUrl = await this.SaveImageAsync(imageFile);
                }

                var newPost = new Posts
                {
                    UserId = userId,
                    Title = dto.Title,
                    Author = dto.Author,
                    DealType = (DealType)dto.DealTypeId,
                    Description = dto.Description,
                    PhotoUrl = savedPhotoUrl,
                    BookGenres = new List<BookGenres>
                    {
                        new BookGenres { GenreId = dto.GenreId },
                    },
                };

                await this.postRepository.AddAsync(newPost);

                this.logger.LogInformation("Successfully created post with ID: {PostId} for user ID: {UserId}", newPost.Id, userId);

                return newPost.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to create post for user ID: {UserId}. Error: {Message}", userId, ex.Message);
                throw;
            }
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                string uploadsFolder = Path.Combine(this.environment.WebRootPath, "images", "posts");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    this.logger.LogInformation("Created directory for post images: {Path}", uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                this.logger.LogInformation("Image saved to disk: {FileName}", uniqueFileName);

                return "/images/posts/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occurred while saving image file.");
                throw new InvalidOperationException("Could not save image file.", ex);
            }
        }
    }
}