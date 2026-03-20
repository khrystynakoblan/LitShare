namespace LitShare.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class EditPostServiceTests
    {
        private readonly Mock<IPostRepository> postRepositoryMock;
        private readonly Mock<ILogger<EditPostService>> loggerMock;
        private readonly EditPostService sut;

        public EditPostServiceTests()
        {
            this.postRepositoryMock = new Mock<IPostRepository>();
            this.loggerMock = new Mock<ILogger<EditPostService>>();

            this.sut = new EditPostService(
                this.postRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_UpdatesBasicFieldsCorrectly()
        {
            var dto = ValidDto();
            var post = ExistingPost();
            this.SetupGetById(post);

            await this.sut.EditPostAsync(dto);

            Assert.Equal(dto.Title, post.Title);
            Assert.Equal(dto.Author, post.Author);
            Assert.Equal(dto.Description, post.Description);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_UpdatesDealTypeCorrectly()
        {
            var dto = ValidDto();
            dto.DealTypeId = (int)DealType.Donation;
            var post = ExistingPost();
            this.SetupGetById(post);

            await this.sut.EditPostAsync(dto);

            Assert.Equal(DealType.Donation, post.DealType);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_UpdatesGenresCorrectly()
        {
            var dto = ValidDto();
            dto.GenreIds = new List<int> { 5, 6 };
            var post = ExistingPost();
            this.SetupGetById(post);

            await this.sut.EditPostAsync(dto);

            Assert.Equal(2, post.BookGenres.Count);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 5);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 6);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_CallsUpdateAsyncOnce()
        {
            this.SetupGetById(ExistingPost());

            await this.sut.EditPostAsync(ValidDto());

            this.postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Posts>()), Times.Once);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_CallsSaveChangesAsyncOnce()
        {
            this.SetupGetById(ExistingPost());

            await this.sut.EditPostAsync(ValidDto());

            this.postRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_LogsStartMessage()
        {
            this.SetupGetById(ExistingPost());

            await this.sut.EditPostAsync(ValidDto());

            this.VerifyLog(LogLevel.Information, "Starting post edit for post ID: 1");
        }

        [Fact]
        public async Task EditPostAsync_ValidData_LogsSuccessMessage()
        {
            this.SetupGetById(ExistingPost());

            await this.sut.EditPostAsync(ValidDto());

            this.VerifyLog(LogLevel.Information, "Successfully edited post with ID: 1");
        }

        [Fact]
        public async Task EditPostAsync_ValidData_ReplacesOldGenresWithNew()
        {
            var post = ExistingPost();
            post.BookGenres.Add(new BookGenres { GenreId = 99 });
            this.SetupGetById(post);

            var dto = ValidDto();
            dto.GenreIds = new List<int> { 7, 8 };

            await this.sut.EditPostAsync(dto);

            Assert.Equal(2, post.BookGenres.Count);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 7);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 8);
        }

        [Fact]
        public async Task EditPostAsync_NullDescription_SavesSuccessfully()
        {
            var dto = ValidDto();
            dto.Description = null;
            this.SetupGetById(ExistingPost());

            await this.sut.EditPostAsync(dto);

            this.postRepositoryMock.Verify(
                r => r.UpdateAsync(It.Is<Posts>(p => p.Description == null)),
                Times.Once);
        }

        [Fact]
        public async Task EditPostAsync_PostNotFound_ThrowsInvalidOperationException()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.EditPostAsync(ValidDto()));
        }

        [Fact]
        public async Task EditPostAsync_PostNotFound_LogsWarning()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.EditPostAsync(ValidDto()));

            this.VerifyLog(LogLevel.Warning, "Post with ID: 1 was not found");
        }

        [Fact]
        public async Task EditPostAsync_PostNotFound_DoesNotCallUpdateOrSave()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.EditPostAsync(ValidDto()));

            this.postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Posts>()), Times.Never);
            this.postRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task EditPostAsync_RepositoryFail_LogsError()
        {
            this.SetupGetById(ExistingPost());
            this.postRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Posts>()))
                .ThrowsAsync(new Exception("DB Crash"));

            await Assert.ThrowsAsync<Exception>(
                () => this.sut.EditPostAsync(ValidDto()));

            this.VerifyLog(LogLevel.Error, "Failed to edit post with ID: 1");
        }

        [Fact]
        public async Task EditPostAsync_RepositoryFail_PropagatesException()
        {
            this.SetupGetById(ExistingPost());
            this.postRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Posts>()))
                .ThrowsAsync(new InvalidOperationException("Fatal error"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.EditPostAsync(ValidDto()));
        }

        private static EditPostDto ValidDto() => new EditPostDto
        {
            PostId = 1,
            Title = "Updated Title",
            Author = "Updated Author",
            Description = "Updated Description",
            GenreIds = new List<int> { 3 },
            DealTypeId = (int)DealType.Exchange,
        };

        private static Posts ExistingPost() => new Posts
        {
            Id = 1,
            Title = "Old Title",
            Author = "Old Author",
            Description = "Old Description",
            DealType = DealType.Exchange,
            BookGenres = new List<BookGenres>(),
        };

        private void SetupGetById(Posts post)
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(post.Id))
                .ReturnsAsync(post);
        }

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