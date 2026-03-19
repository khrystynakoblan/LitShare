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

    public class CreatePostServiceTests
    {
        private readonly Mock<IPostRepository> postRepositoryMock;
        private readonly Mock<IWebHostEnvironment> environmentMock;
        private readonly Mock<ILogger<CreatePostService>> loggerMock;
        private readonly CreatePostService sut;

        public CreatePostServiceTests()
        {
            this.postRepositoryMock = new Mock<IPostRepository>();
            this.environmentMock = new Mock<IWebHostEnvironment>();
            this.loggerMock = new Mock<ILogger<CreatePostService>>();

            this.environmentMock.Setup(m => m.WebRootPath).Returns("C:/fake_wwwroot");

            this.sut = new CreatePostService(
                this.postRepositoryMock.Object,
                this.environmentMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_ReturnsCorrectPostId()
        {
            var dto = ValidDto();
            int expectedId = 777;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => p.Id = expectedId)
                .Returns(Task.CompletedTask);

            var result = await this.sut.CreatePostAsync(dto, null, 1);

            Assert.Equal(expectedId, result);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_MapsBasicFieldsCorrectly()
        {
            var dto = ValidDto();
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, null, 99);

            Assert.NotNull(capturedPost);
            Assert.Equal(dto.Title, capturedPost!.Title);
            Assert.Equal(dto.Author, capturedPost.Author);
            Assert.Equal(99, capturedPost.UserId);
        }

        [Fact]
        public async Task CreatePostAsync_WithImage_GeneratesAndSetsPhotoUrl()
        {
            var dto = ValidDto();
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("fake content"));

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns("test_image.png");
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, fileMock.Object, 1);

            Assert.NotNull(capturedPost?.PhotoUrl);
            Assert.Contains("/images/posts/", capturedPost!.PhotoUrl);
            Assert.EndsWith(".png", capturedPost.PhotoUrl);
        }

        [Fact]
        public async Task CreatePostAsync_NoImage_SetsPhotoUrlToNull()
        {
            var dto = ValidDto();
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, null, 1);

            Assert.Null(capturedPost!.PhotoUrl);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_CreatesGenreRelation()
        {
            var dto = ValidDto();
            dto.GenreId = 15;
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, null, 1);

            var genreRelation = Assert.Single(capturedPost!.BookGenres);
            Assert.Equal(15, genreRelation.GenreId);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_MapsDealTypeCorrectly()
        {
            var dto = ValidDto();
            dto.DealTypeId = (int)DealType.Exchange;
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, null, 1);

            Assert.Equal(DealType.Exchange, capturedPost!.DealType);
        }

        [Fact]
        public async Task CreatePostAsync_Success_LogsSuccessMessage()
        {
            var dto = ValidDto();
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, null, 1);

            this.VerifyLog(LogLevel.Information, "Successfully created post");
        }

        [Fact]
        public async Task CreatePostAsync_RepositoryFail_LogsErrorAndThrows()
        {
            var dto = ValidDto();
            var exception = new Exception("Critical Database Error");
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>())).ThrowsAsync(exception);

            await Assert.ThrowsAsync<Exception>(() => this.sut.CreatePostAsync(dto, null, 1));
            this.VerifyLog(LogLevel.Error, "Failed to create post");
        }

        private static CreatePostDto ValidDto() => new CreatePostDto
        {
            Title = "Clean Architecture",
            Author = "Robert C. Martin",
            Description = "A handbook of agile software craftsmanship.",
            GenreId = 1,
            DealTypeId = 1
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