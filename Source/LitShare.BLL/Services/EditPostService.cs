namespace LitShare.BLL.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class EditPostService : IEditPostService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<EditPostService> logger;

        public EditPostService(IPostRepository postRepository, ILogger<EditPostService> logger)
        {
            this.postRepository = postRepository;
            this.logger = logger;
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
    }
}