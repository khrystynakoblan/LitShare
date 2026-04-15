using LitShare.BLL.Common;
using LitShare.BLL.DTOs;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace LitShare.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<PostService> logger;

        public PostService(IPostRepository postRepository, ILogger<PostService> logger)
        {
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<PostCardDto>>> GetPostsByUserIdAsync(int userId, bool? isActive)
        {
            this.logger.LogInformation("Fetching posts for UserId: {UserId} with filter IsActive: {IsActive}", userId, isActive);

            var posts = await this.postRepository.GetByUserIdAsync(userId);

            if (isActive != null)
            {
                posts = posts.Where(p => p.IsActive == isActive);
            }

            var dtos = posts.Select(p => new PostCardDto
            {
                Id = p.Id,
                Title = p.Title ?? string.Empty,
                Author = p.Author ?? string.Empty,
                City = p.User?.City ?? "Не вказано",
                PhotoUrl = p.PhotoUrl,
                IsActive = p.IsActive
            }).ToList();

            return dtos;
        }

        public async Task<Result<PostDetailsDto>> GetPostDetailsAsync(int id)
        {
            this.logger.LogInformation("Fetching post details for ID: {Id}", id);

            var post = await this.postRepository.GetByIdWithGenresAsync(id);

            if (post == null)
            {
                this.logger.LogWarning("Post not found. ID: {Id}", id);
                return Result<PostDetailsDto>.Failure($"Post with ID {id} not found");
            }

            this.logger.LogInformation("Successfully fetched post details for ID: {Id}", id);

            return new PostDetailsDto
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Author = post.Author ?? string.Empty,
                Description = post.Description,
                PhotoUrl = post.PhotoUrl,
                DealType = post.DealType,
                Genres = post.BookGenres
                    .Select(bg => bg.Genre?.Name ?? string.Empty)
                    .ToList(),
                UserId = post.UserId,
            };
        }

        public async Task<Result<bool>> SetPostStatusAsync(int postId, int userId, bool isActive)
        {
            this.logger.LogInformation("Changing post status. PostId: {PostId}, UserId: {UserId}, NewStatus: {Status}", postId, userId, isActive);

            var post = await this.postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                this.logger.LogWarning("Post not found. PostId: {PostId}", postId);
                return Result<bool>.Failure("Пост не знайдено");
            }

            if (post.UserId != userId)
            {
                this.logger.LogWarning("Unauthorized access. UserId: {UserId}, PostId: {PostId}", userId, postId);
                return Result<bool>.Failure("Немає доступу до цього поста");
            }

            post.IsActive = isActive;

            await this.postRepository.UpdateAsync(post);

            this.logger.LogInformation("Post status updated successfully. PostId: {PostId}", postId);

            return true;
        }

        public async Task<Result<bool>> DeletePostAsync(int postId, int userId)
        {
            this.logger.LogInformation("Deleting post. PostId: {PostId}, UserId: {UserId}", postId, userId);

            var post = await this.postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                this.logger.LogWarning("Post not found. PostId: {PostId}", postId);
                return Result<bool>.Failure("Пост не знайдено");
            }

            if (post.UserId != userId)
            {
                this.logger.LogWarning("Unauthorized delete attempt. UserId: {UserId}, PostId: {PostId}", userId, postId);
                return Result<bool>.Failure("Немає доступу до цього поста");
            }

            await this.postRepository.DeleteAsync(post);
            await this.postRepository.SaveChangesAsync();

            this.logger.LogInformation("Post deleted successfully. PostId: {PostId}", postId);

            return true;
        }
    }
}