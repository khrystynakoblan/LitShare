namespace LitShare.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Hosting;
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

            this.environmentMock.Setup(e => e.WebRootPath).Returns("C:/fake_wwwroot");

            this.sut = new CreatePostService(
                this.postRepositoryMock.Object,
                this.environmentMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_ReturnsSuccessWithCorrectPostId()
        {
            var dto = ValidDto();
            int expectedId = 777;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => p.Id = expectedId)
                .Returns(Task.CompletedTask);

            var result = await this.sut.CreatePostAsync(dto, 1);

            Assert.True(result.IsSuccess); 
            Assert.Equal(expectedId, result.Value);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_MapsBasicFieldsCorrectly()
        {
            var dto = ValidDto();
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            var result = await this.sut.CreatePostAsync(dto, 99);

            Assert.True(result.IsSuccess);
            Assert.NotNull(capturedPost);
            Assert.Equal(dto.Title, capturedPost!.Title);
            Assert.Equal(99, capturedPost.UserId);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_MapsDealTypeEnumCorrectly()
        {
            var dto = ValidDto();
            dto.DealTypeId = (int)DealType.Donation;
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, 1);

            Assert.Equal(DealType.Donation, capturedPost!.DealType);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_CreatesGenreRelations()
        {
            var dto = ValidDto();
            dto.GenreIds = new List<int> { 12, 13 };
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, 1);

            Assert.Equal(2, capturedPost!.BookGenres.Count);
            Assert.Contains(capturedPost.BookGenres, bg => bg.GenreId == 12);
            Assert.Contains(capturedPost.BookGenres, bg => bg.GenreId == 13);
        }

        [Fact]
        public async Task CreatePostAsync_NoImageFile_SetsPhotoUrlToNull()
        {
            var dto = ValidDto();
            dto.ImageFile = null;
            Posts? capturedPost = null;
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .Callback<Posts>(p => capturedPost = p)
                .Returns(Task.CompletedTask);

            await this.sut.CreatePostAsync(dto, 1);

            Assert.Null(capturedPost!.PhotoUrl);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_CallsSaveAsyncExactlyOnce()
        {
            await this.sut.CreatePostAsync(ValidDto(), 1);

            this.postRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreatePostAsync_ValidData_LogsStartMessage()
        {
            var dto = ValidDto();

            await this.sut.CreatePostAsync(dto, 1);

            this.VerifyLog(LogLevel.Information, $"Starting post creation for user ID: 1. Title: {dto.Title}");
        }

        [Fact]
        public async Task CreatePostAsync_Success_LogsSuccessMessage()
        {
            await this.sut.CreatePostAsync(ValidDto(), 1);

            this.VerifyLog(LogLevel.Information, "Successfully created post");
        }

        [Fact]
        public async Task CreatePostAsync_RepositoryFail_PropagatesException()
        {
            this.postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Posts>()))
                .ThrowsAsync(new InvalidOperationException("Fatal error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.sut.CreatePostAsync(ValidDto(), 1));
        }

        private static CreatePostDto ValidDto() => new CreatePostDto
        {
            Title = "Test Book",
            Author = "Test Author",
            Description = "Test Description",
            GenreIds = new List<int> { 1 },
            DealTypeId = (int)DealType.Exchange,
            ImageFile = null,
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