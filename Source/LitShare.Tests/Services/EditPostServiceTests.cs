namespace LitShare.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class EditPostServiceTests
    {
        private readonly Mock<IPostRepository> postRepositoryMock;
        private readonly Mock<IWebHostEnvironment> environmentMock;
        private readonly Mock<ILogger<EditPostService>> loggerMock;
        private readonly EditPostService sut;

        private const int CurrentUserId = 10;

        public EditPostServiceTests()
        {
            this.postRepositoryMock = new Mock<IPostRepository>();
            this.environmentMock = new Mock<IWebHostEnvironment>();
            this.loggerMock = new Mock<ILogger<EditPostService>>();

            this.environmentMock.Setup(e => e.WebRootPath).Returns("C:/fake_wwwroot");

            this.sut = new EditPostService(
                this.postRepositoryMock.Object,
                this.environmentMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_UpdatesBasicFieldsCorrectly()
        {
            var dto = ValidDto();
            var post = ExistingPost(CurrentUserId);
            this.SetupGetById(post);

            var result = await this.sut.EditPostAsync(dto, CurrentUserId);

            Assert.True(result.IsSuccess);
            Assert.Equal(dto.Title, post.Title);
            Assert.Equal(dto.Author, post.Author);
            Assert.Equal(dto.Description, post.Description);
        }

        [Fact]
        public async Task EditPostAsync_WithNewPhoto_UpdatesPhotoUrl()
        {
            var post = ExistingPost(CurrentUserId);
            this.SetupGetById(post);

            var fileMock = new Mock<IFormFile>();
            var content = "fake image content";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var dto = ValidDto();
            dto.NewPhoto = fileMock.Object;

            var result = await this.sut.EditPostAsync(dto, CurrentUserId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(post.PhotoUrl);
            Assert.Contains("/images/posts/", post.PhotoUrl);
            Assert.EndsWith(".jpg", post.PhotoUrl);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_UpdatesDealTypeCorrectly()
        {
            var dto = ValidDto();
            dto.DealTypeId = (int)DealType.Donation;
            var post = ExistingPost(CurrentUserId);
            this.SetupGetById(post);

            var result = await this.sut.EditPostAsync(dto, CurrentUserId);

            Assert.True(result.IsSuccess);
            Assert.Equal(DealType.Donation, post.DealType);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_UpdatesGenresCorrectly()
        {
            var dto = ValidDto();
            dto.GenreIds = new List<int> { 5, 6 };
            var post = ExistingPost(CurrentUserId);
            this.SetupGetById(post);

            var result = await this.sut.EditPostAsync(dto, CurrentUserId);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, post.BookGenres.Count);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 5);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 6);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_CallsUpdateAsyncOnce()
        {
            this.SetupGetById(ExistingPost(CurrentUserId));

            await this.sut.EditPostAsync(ValidDto(), CurrentUserId);

            this.postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Posts>()), Times.Once);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_CallsSaveChangesAsyncOnce()
        {
            this.SetupGetById(ExistingPost(CurrentUserId));

            await this.sut.EditPostAsync(ValidDto(), CurrentUserId);

            this.postRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task EditPostAsync_ValidData_LogsStartMessage()
        {
            this.SetupGetById(ExistingPost(CurrentUserId));

            await this.sut.EditPostAsync(ValidDto(), CurrentUserId);

            this.VerifyLog(LogLevel.Information, "Starting post edit for post ID: 1");
        }

        [Fact]
        public async Task EditPostAsync_ValidData_LogsSuccessMessage()
        {
            this.SetupGetById(ExistingPost(CurrentUserId));

            await this.sut.EditPostAsync(ValidDto(), CurrentUserId);

            this.VerifyLog(LogLevel.Information, "Successfully edited post with ID: 1");
        }

        [Fact]
        public async Task EditPostAsync_ValidData_ReplacesOldGenresWithNew()
        {
            var post = ExistingPost(CurrentUserId);
            post.BookGenres.Add(new BookGenres { GenreId = 99 });
            this.SetupGetById(post);

            var dto = ValidDto();
            dto.GenreIds = new List<int> { 7, 8 };

            var result = await this.sut.EditPostAsync(dto, CurrentUserId);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, post.BookGenres.Count);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 7);
            Assert.Contains(post.BookGenres, bg => bg.GenreId == 8);
        }

        [Fact]
        public async Task EditPostAsync_NullDescription_SavesSuccessfully()
        {
            var dto = ValidDto();
            dto.Description = null;
            this.SetupGetById(ExistingPost(CurrentUserId));

            var result = await this.sut.EditPostAsync(dto, CurrentUserId);

            Assert.True(result.IsSuccess);
            this.postRepositoryMock.Verify(
                r => r.UpdateAsync(It.Is<Posts>(p => p.Description == null)),
                Times.Once);
        }

        [Fact]
        public async Task EditPostAsync_OtherUserPost_ReturnsUnauthorized()
        {
            var dto = ValidDto();
            var post = ExistingPost(999);
            this.SetupGetById(post);

            var result = await this.sut.EditPostAsync(dto, CurrentUserId);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsUnauthorized);
            this.postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Posts>()), Times.Never);
        }

        [Fact]
        public async Task EditPostAsync_PostNotFound_ReturnsFailureResult()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            var result = await this.sut.EditPostAsync(ValidDto(), CurrentUserId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Оголошення не знайдено.", result.Error);
        }

        [Fact]
        public async Task EditPostAsync_RepositoryFail_PropagatesException()
        {
            this.SetupGetById(ExistingPost(CurrentUserId));
            this.postRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Posts>()))
                .ThrowsAsync(new InvalidOperationException("Fatal error"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.EditPostAsync(ValidDto(), CurrentUserId));
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

        private static Posts ExistingPost(int userId) => new Posts
        {
            Id = 1,
            UserId = userId,
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