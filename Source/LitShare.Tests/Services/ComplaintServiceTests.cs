namespace LitShare.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class ComplaintServiceTests
    {
        private readonly Mock<IComplaintRepository> complaintRepositoryMock;
        private readonly Mock<ILogger<ComplaintService>> loggerMock;
        private readonly ComplaintService sut;

        private const int CurrentUserId = 10;
        private const int PostId = 5;

        public ComplaintServiceTests()
        {
            this.complaintRepositoryMock = new Mock<IComplaintRepository>();
            this.loggerMock = new Mock<ILogger<ComplaintService>>();

            this.sut = new ComplaintService(
                this.complaintRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_ReturnsSuccess()
        {
            var result = await this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_CallsAddAsyncOnce()
        {
            await this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId);

            this.complaintRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Complaints>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_CallsSaveChangesAsyncOnce()
        {
            await this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId);

            this.complaintRepositoryMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_SetsCorrectPostId()
        {
            var dto = ValidDto();

            await this.sut.CreateComplaintAsync(dto, CurrentUserId);

            this.complaintRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Complaints>(c => c.PostId == PostId)),
                Times.Once);
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_SetsCorrectComplainantId()
        {
            await this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId);

            this.complaintRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Complaints>(c => c.ComplainantId == CurrentUserId)),
                Times.Once);
        }

        [Fact]
        public async Task CreateComplaintAsync_WithoutAdditionalText_TextEqualsComplaintType()
        {
            var dto = ValidDto();
            dto.AdditionalText = null;

            await this.sut.CreateComplaintAsync(dto, CurrentUserId);

            this.complaintRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Complaints>(c => c.Text == dto.ComplaintType)),
                Times.Once);
        }

        [Fact]
        public async Task CreateComplaintAsync_WithAdditionalText_TextContainsBothParts()
        {
            var dto = ValidDto();
            dto.AdditionalText = "Додатковий коментар";

            await this.sut.CreateComplaintAsync(dto, CurrentUserId);

            this.complaintRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Complaints>(c =>
                    c.Text != null &&
                    c.Text.Contains(dto.ComplaintType) &&
                    c.Text.Contains(dto.AdditionalText))),
                Times.Once);
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_DateIsSet()
        {
            var before = DateTime.UtcNow;

            await this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId);

            this.complaintRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Complaints>(c => c.Date >= before)),
                Times.Once);
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_LogsStartMessage()
        {
            await this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId);

            this.VerifyLog(LogLevel.Information, $"User {CurrentUserId} creating complaint for post {PostId}");
        }

        [Fact]
        public async Task CreateComplaintAsync_ValidData_LogsSuccessMessage()
        {
            await this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId);

            this.VerifyLog(LogLevel.Information, $"Complaint successfully created for post {PostId}");
        }

        [Fact]
        public async Task CreateComplaintAsync_RepositoryFails_PropagatesException()
        {
            this.complaintRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Complaints>()))
                .ThrowsAsync(new InvalidOperationException("DB error"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId));
        }

        [Fact]
        public async Task CreateComplaintAsync_SaveChangesFails_PropagatesException()
        {
            this.complaintRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new InvalidOperationException("Save failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.CreateComplaintAsync(ValidDto(), CurrentUserId));
        }

        private static CreateComplaintDto ValidDto() => new CreateComplaintDto
        {
            PostId = PostId,
            ComplaintType = "Неправдива інформація в оголошенні",
            AdditionalText = null,
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
    }
}