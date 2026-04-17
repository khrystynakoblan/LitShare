namespace LitShare.BLL.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;

    public interface IReviewService
    {
        Task<Result<bool>> CreateReviewAsync(CreateReviewDto dto, int reviewerId);

        Task<Result<IEnumerable<ReviewDto>>> GetReviewsByUserIdAsync(int reviewedUserId);

        Task<Result<bool>> HasReviewedAsync(int reviewerId, int reviewedUserId);

        Task<Result<ReviewDto>> GetByIdAsync(int reviewId);

        Task<Result<bool>> EditReviewAsync(EditReviewDto dto, int reviewerId);
    }
}