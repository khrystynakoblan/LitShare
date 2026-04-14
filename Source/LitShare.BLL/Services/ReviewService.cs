namespace LitShare.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository reviewRepository;
        private readonly ILogger<ReviewService> logger;

        public ReviewService(
            IReviewRepository reviewRepository,
            ILogger<ReviewService> logger)
        {
            this.reviewRepository = reviewRepository;
            this.logger = logger;
        }

        public async Task<Result<bool>> CreateReviewAsync(CreateReviewDto dto, int reviewerId)
        {
            this.logger.LogInformation("User {ReviewerId} creating review for user {ReviewedUserId}", reviewerId, dto.ReviewedUserId);

            var alreadyExists = await this.reviewRepository.ExistsAsync(reviewerId, dto.ReviewedUserId);

            if (alreadyExists)
            {
                return Result<bool>.Failure("Ви вже залишали відгук цьому користувачу.");
            }

            var review = new Reviews
            {
                ReviewerId = reviewerId,
                ReviewedUserId = dto.ReviewedUserId,
                Rating = dto.Rating,
                Text = dto.Text,
                Date = DateTime.UtcNow,
            };

            await this.reviewRepository.AddAsync(review);
            await this.reviewRepository.SaveChangesAsync();

            this.logger.LogInformation("Review successfully created for user {ReviewedUserId} by user {ReviewerId}", dto.ReviewedUserId, reviewerId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsByUserIdAsync(int reviewedUserId)
        {
            var reviews = await this.reviewRepository.GetByReviewedUserIdAsync(reviewedUserId);

            var dtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                Rating = r.Rating,
                Date = r.Date,
                ReviewerId = r.ReviewerId,
                ReviewerName = r.Reviewer?.Name ?? "Користувач",
                ReviewerPhotoUrl = r.Reviewer?.PhotoUrl,
            });

            return Result<IEnumerable<ReviewDto>>.Success(dtos);
        }

        public async Task<Result<bool>> HasReviewedAsync(int reviewerId, int reviewedUserId)
        {
            var exists = await this.reviewRepository.ExistsAsync(reviewerId, reviewedUserId);
            return Result<bool>.Success(exists);
        }
    }
}