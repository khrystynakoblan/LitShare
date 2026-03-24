namespace LitShare.BLL.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class EditPostService : IEditPostService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<EditPostService> logger;

        public EditPostService(
            IPostRepository postRepository,
            ILogger<EditPostService> logger)
        {
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<Result<PostViewDto>> GetPostByIdAsync(int id, int currentUserId)
        {
            this.logger.LogInformation("Fetching post with ID: {Id} for User: {UserId}", id, currentUserId);
            var post = await this.postRepository.GetByIdAsync(id);

            if (post == null)
            {
                this.logger.LogWarning("Post not found. ID: {Id}", id);
                return Result<PostViewDto>.Failure("Оголошення не знайдено.");
            }

            if (post.UserId != currentUserId)
            {
                this.logger.LogWarning("User {UserId} attempted to access post {PostId} belonging to user {OwnerId}", currentUserId, id, post.UserId);
                return Result<PostViewDto>.Unauthorized("У вас немає прав для редагування цього оголошення.");
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

            return Result<PostViewDto>.Success(dto);
        }

        public async Task<Result<bool>> EditPostAsync(EditPostDto dto, int currentUserId)
        {
            this.logger.LogInformation("Starting post edit for post ID: {PostId} by User: {UserId}", dto.PostId, currentUserId);

            var post = await this.postRepository.GetByIdAsync(dto.PostId);

            if (post == null)
            {
                this.logger.LogWarning("Post with ID: {PostId} was not found", dto.PostId);
                return Result<bool>.Failure("Оголошення не знайдено.");
            }

            if (post.UserId != currentUserId)
            {
                this.logger.LogWarning("User {UserId} attempted to edit post {PostId} belonging to user {OwnerId}", currentUserId, dto.PostId, post.UserId);
                return Result<bool>.Unauthorized("У вас немає прав для редагування цього оголошення.");
            }

            post.Title = dto.Title;
            post.Author = dto.Author;
            post.Description = dto.Description;
            post.DealType = (DealType)dto.DealTypeId;

            post.BookGenres.Clear();
            foreach (var genreId in dto.GenreIds)
            {
                post.BookGenres.Add(new BookGenres { GenreId = genreId });
            }

            await this.postRepository.UpdateAsync(post);
            await this.postRepository.SaveChangesAsync();

            this.logger.LogInformation("Successfully edited post with ID: {PostId}", dto.PostId);
            return Result<bool>.Success(true);
        }
    }
}