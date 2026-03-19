namespace LitShare.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class CreatePostService : ICreatePostService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<CreatePostService> logger;

        public CreatePostService(IPostRepository postRepository, ILogger<CreatePostService> logger)
        {
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<int> CreatePostAsync(CreatePostDto dto, int userId)
        {
            this.logger.LogInformation("Starting post creation for user ID: {UserId}. Title: {Title}", userId, dto.Title);

            try
            {
                var newPost = new Posts
                {
                    UserId = userId,
                    Title = dto.Title,
                    Author = dto.Author,
                    DealType = (DealType)dto.DealTypeId,
                    Description = dto.Description,
                    PhotoUrl = dto.PhotoUrl,
                    BookGenres = new List<BookGenres>
                    {
                        new BookGenres { GenreId = dto.GenreId },
                    },
                };

                await this.postRepository.AddAsync(newPost);
                await this.postRepository.SaveChangesAsync();

                this.logger.LogInformation("Successfully created post with ID: {PostId} for user ID: {UserId}", newPost.Id, userId);

                return newPost.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to create post for user ID: {UserId}. Error: {Message}", userId, ex.Message);
                throw;
            }
        }
    }
}