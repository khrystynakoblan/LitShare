namespace LitShare.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> reviewRepositoryMock;
        private readonly Mock<ILogger<ReviewService>> loggerMock;
        private readonly ReviewService sut;

        private const int ReviewerId = 10;
        private const int ReviewedUserId = 20;

        public ReviewServiceTests()
        {
            this.reviewRepositoryMock = new Mock<IReviewRepository>();
            this.loggerMock = new Mock<ILogger<ReviewService>>();

            this.sut = new ReviewService(
                this.reviewRepositoryMock.Object,
                this.loggerMock.Object);
        }


        [Fact]
        public async Task CreateReviewAsync_ValidData_ReturnsSuccess()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            var result = await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task CreateReviewAsync_AlreadyReviewed_ReturnsFailure()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(true);

            var result = await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Ви вже залишали відгук цьому користувачу.", result.Error);
        }

        [Fact]
        public async Task CreateReviewAsync_AlreadyReviewed_DoesNotCallAddAsync()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(true);

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Reviews>()), Times.Never);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_CallsAddAsyncOnce()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Reviews>()), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_CallsSaveChangesAsyncOnce()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_SetsCorrectReviewerId()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.Is<Reviews>(r => r.ReviewerId == ReviewerId)), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_SetsCorrectReviewedUserId()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.Is<Reviews>(r => r.ReviewedUserId == ReviewedUserId)), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_SetsCorrectRating()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);
            var dto = ValidDto();

            await this.sut.CreateReviewAsync(dto, ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.Is<Reviews>(r => r.Rating == dto.Rating)), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_WithText_SetsCorrectText()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);
            var dto = ValidDto();
            dto.Text = "Чудовий користувач!";

            await this.sut.CreateReviewAsync(dto, ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.Is<Reviews>(r => r.Text == dto.Text)), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_WithoutText_TextIsNull()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);
            var dto = ValidDto();
            dto.Text = null;

            await this.sut.CreateReviewAsync(dto, ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.Is<Reviews>(r => r.Text == null)), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_DateIsSet()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);
            var before = DateTime.UtcNow;

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.AddAsync(It.Is<Reviews>(r => r.Date >= before)), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_LogsStartMessage()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.VerifyLog(LogLevel.Information, $"User {ReviewerId} creating review for user {ReviewedUserId}");
        }

        [Fact]
        public async Task CreateReviewAsync_ValidData_LogsSuccessMessage()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            await this.sut.CreateReviewAsync(ValidDto(), ReviewerId);

            this.VerifyLog(LogLevel.Information, $"Review successfully created for user {ReviewedUserId} by user {ReviewerId}");
        }

        [Fact]
        public async Task CreateReviewAsync_RepositoryFails_PropagatesException()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);
            this.reviewRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Reviews>())).ThrowsAsync(new InvalidOperationException("DB error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.sut.CreateReviewAsync(ValidDto(), ReviewerId));
        }


        [Fact]
        public async Task GetReviewsByUserIdAsync_WithReviews_ReturnsSuccess()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByReviewedUserIdAsync(ReviewedUserId)).ReturnsAsync(SampleReviews());

            var result = await this.sut.GetReviewsByUserIdAsync(ReviewedUserId);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetReviewsByUserIdAsync_WithReviews_ReturnsCorrectCount()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByReviewedUserIdAsync(ReviewedUserId)).ReturnsAsync(SampleReviews());

            var result = await this.sut.GetReviewsByUserIdAsync(ReviewedUserId);

            Assert.Equal(2, result.Value!.Count());
        }

        [Fact]
        public async Task GetReviewsByUserIdAsync_WithReviews_MapsRatingCorrectly()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByReviewedUserIdAsync(ReviewedUserId)).ReturnsAsync(SampleReviews());

            var result = await this.sut.GetReviewsByUserIdAsync(ReviewedUserId);

            Assert.All(result.Value!, r => Assert.True(r.Rating > 0));
        }

        [Fact]
        public async Task GetReviewsByUserIdAsync_ReviewerWithNoName_UsesDefaultName()
        {
            var reviews = new List<Reviews>
            {
                new Reviews
                {
                    Id = 1,
                    Rating = 4,
                    ReviewerId = ReviewerId,
                    ReviewedUserId = ReviewedUserId,
                    Date = DateTime.UtcNow,
                    Reviewer = new Users { Name = null },
                },
            };

            this.reviewRepositoryMock.Setup(r => r.GetByReviewedUserIdAsync(ReviewedUserId)).ReturnsAsync(reviews);

            var result = await this.sut.GetReviewsByUserIdAsync(ReviewedUserId);

            Assert.Equal("Користувач", result.Value!.First().ReviewerName);
        }

        [Fact]
        public async Task GetReviewsByUserIdAsync_EmptyList_ReturnsSuccessWithEmptyCollection()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByReviewedUserIdAsync(ReviewedUserId)).ReturnsAsync(new List<Reviews>());

            var result = await this.sut.GetReviewsByUserIdAsync(ReviewedUserId);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }


        [Fact]
        public async Task HasReviewedAsync_ReviewExists_ReturnsTrue()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(true);

            var result = await this.sut.HasReviewedAsync(ReviewerId, ReviewedUserId);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task HasReviewedAsync_ReviewNotExists_ReturnsFalse()
        {
            this.reviewRepositoryMock.Setup(r => r.ExistsAsync(ReviewerId, ReviewedUserId)).ReturnsAsync(false);

            var result = await this.sut.HasReviewedAsync(ReviewerId, ReviewedUserId);

            Assert.True(result.IsSuccess);
            Assert.False(result.Value);
        }

        private static CreateReviewDto ValidDto() => new CreateReviewDto
        {
            ReviewedUserId = ReviewedUserId,
            Rating = 5,
            Text = null,
        };

        private static List<Reviews> SampleReviews() => new List<Reviews>
        {
            new Reviews
            {
                Id = 1,
                Rating = 5,
                Text = "Чудово!",
                ReviewerId = ReviewerId,
                ReviewedUserId = ReviewedUserId,
                Date = DateTime.UtcNow,
                Reviewer = new Users { Name = "Іванка", PhotoUrl = null },
            },
            new Reviews
            {
                Id = 2,
                Rating = 4,
                Text = null,
                ReviewerId = 11,
                ReviewedUserId = ReviewedUserId,
                Date = DateTime.UtcNow,
                Reviewer = new Users { Name = "Олег", PhotoUrl = null },
            },
        };

        private void VerifyLog(LogLevel level, string messagePart)
        {
            this.loggerMock.Verify(
                l => l.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(messagePart)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        private static EditReviewDto ValidEditDto() => new EditReviewDto
        {
            ReviewId = 1,
            Rating = 4,
            Text = "Оновлений відгук",
        };

        private static Reviews SampleReview() => new Reviews
        {
            Id = 1,
            Rating = 5,
            Text = "Старий відгук",
            ReviewerId = ReviewerId,
            ReviewedUserId = ReviewedUserId,
            Date = DateTime.UtcNow,
        };


        [Fact]
        public async Task GetByIdAsync_ReviewExists_ReturnsSuccess()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await this.sut.GetByIdAsync(review.Id);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_ReviewExists_MapsFieldsCorrectly()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await this.sut.GetByIdAsync(review.Id);

            Assert.Equal(review.Id, result.Value!.Id);
            Assert.Equal(review.Rating, result.Value.Rating);
            Assert.Equal(review.ReviewerId, result.Value.ReviewerId);
            Assert.Equal(review.ReviewedUserId, result.Value.ReviewedUserId);
        }

        [Fact]
        public async Task GetByIdAsync_ReviewNotFound_ReturnsFailure()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Reviews?)null);

            var result = await this.sut.GetByIdAsync(999);

            Assert.False(result.IsSuccess);
            Assert.Equal("Відгук не знайдено.", result.Error);
        }


        [Fact]
        public async Task EditReviewAsync_ValidData_ReturnsSuccess()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await this.sut.EditReviewAsync(ValidEditDto(), ReviewerId);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task EditReviewAsync_ReviewNotFound_ReturnsFailure()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Reviews?)null);

            var result = await this.sut.EditReviewAsync(ValidEditDto(), ReviewerId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Відгук не знайдено.", result.Error);
        }

        [Fact]
        public async Task EditReviewAsync_WrongUser_ReturnsFailure()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await this.sut.EditReviewAsync(ValidEditDto(), 999);

            Assert.False(result.IsSuccess);
            Assert.Equal("Ви не можете редагувати чужий відгук.", result.Error);
        }

        [Fact]
        public async Task EditReviewAsync_WrongUser_DoesNotCallUpdate()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.EditReviewAsync(ValidEditDto(), 999);

            this.reviewRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Reviews>()), Times.Never);
        }

        [Fact]
        public async Task EditReviewAsync_ValidData_UpdatesRating()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);
            var dto = ValidEditDto();
            dto.Rating = 3;

            await this.sut.EditReviewAsync(dto, ReviewerId);

            Assert.Equal(3, review.Rating);
        }

        [Fact]
        public async Task EditReviewAsync_ValidData_UpdatesText()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);
            var dto = ValidEditDto();
            dto.Text = "Оновлений коментар";

            await this.sut.EditReviewAsync(dto, ReviewerId);

            Assert.Equal("Оновлений коментар", review.Text);
        }

        [Fact]
        public async Task EditReviewAsync_ValidData_CallsUpdateAsyncOnce()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.EditReviewAsync(ValidEditDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Reviews>()), Times.Once);
        }

        [Fact]
        public async Task EditReviewAsync_ValidData_CallsSaveChangesAsyncOnce()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.EditReviewAsync(ValidEditDto(), ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task EditReviewAsync_ValidData_LogsStartMessage()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.EditReviewAsync(ValidEditDto(), ReviewerId);

            this.VerifyLog(LogLevel.Information, $"User {ReviewerId} editing review {review.Id}");
        }

        [Fact]
        public async Task EditReviewAsync_ValidData_LogsSuccessMessage()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.EditReviewAsync(ValidEditDto(), ReviewerId);

            this.VerifyLog(LogLevel.Information, $"Review {review.Id} successfully edited by user {ReviewerId}");
        }

        [Fact]
        public async Task EditReviewAsync_RepositoryFails_PropagatesException()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);
            this.reviewRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Reviews>())).ThrowsAsync(new InvalidOperationException("DB error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.sut.EditReviewAsync(ValidEditDto(), ReviewerId));
        }

        [Fact]
        public async Task DeleteReviewAsync_ValidData_ReturnsSuccess()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await this.sut.DeleteReviewAsync(review.Id, ReviewerId);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task DeleteReviewAsync_ReviewNotFound_ReturnsFailure()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Reviews?)null);

            var result = await this.sut.DeleteReviewAsync(999, ReviewerId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Відгук не знайдено.", result.Error);
        }

        [Fact]
        public async Task DeleteReviewAsync_WrongUser_ReturnsFailure()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await this.sut.DeleteReviewAsync(review.Id, 999);

            Assert.False(result.IsSuccess);
            Assert.Equal("Ви не можете видалити чужий відгук.", result.Error);
        }

        [Fact]
        public async Task DeleteReviewAsync_WrongUser_DoesNotCallDelete()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.DeleteReviewAsync(review.Id, 999);

            this.reviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Reviews>()), Times.Never);
        }

        [Fact]
        public async Task DeleteReviewAsync_ReviewNotFound_DoesNotCallDelete()
        {
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Reviews?)null);

            await this.sut.DeleteReviewAsync(999, ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Reviews>()), Times.Never);
        }

        [Fact]
        public async Task DeleteReviewAsync_ValidData_CallsDeleteAsyncOnce()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.DeleteReviewAsync(review.Id, ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Reviews>()), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_ValidData_CallsSaveChangesAsyncOnce()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.DeleteReviewAsync(review.Id, ReviewerId);

            this.reviewRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_ValidData_LogsStartMessage()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.DeleteReviewAsync(review.Id, ReviewerId);

            this.VerifyLog(LogLevel.Information, $"User {ReviewerId} deleting review {review.Id}");
        }

        [Fact]
        public async Task DeleteReviewAsync_ValidData_LogsSuccessMessage()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);

            await this.sut.DeleteReviewAsync(review.Id, ReviewerId);

            this.VerifyLog(LogLevel.Information, $"Review {review.Id} successfully deleted by user {ReviewerId}");
        }

        [Fact]
        public async Task DeleteReviewAsync_RepositoryFails_PropagatesException()
        {
            var review = SampleReview();
            this.reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);
            this.reviewRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Reviews>())).ThrowsAsync(new InvalidOperationException("DB error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.sut.DeleteReviewAsync(review.Id, ReviewerId));
        }
    }
}