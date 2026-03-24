using LitShare.BLL.Services;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LitShare.Tests.Services
{
    public class ProfileServiceTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<ILogger<ProfileService>> loggerMock;
        private readonly ProfileService sut;

        public ProfileServiceTests()
        {
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.loggerMock = new Mock<ILogger<ProfileService>>();

            this.sut = new ProfileService(
                this.userRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsSuccessWithUser()
        {
            var user = new Users { Id = 1, Name = "Test" };

            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var result = await this.sut.GetUserByIdAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Test", result.Value.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Users?)null);

            var result = await this.sut.GetUserByIdAsync(1);

            Assert.False(result.IsSuccess);
            Assert.Equal("Користувача не знайдено.", result.Error);
        }

        [Fact]
        public async Task GetUserByIdAsync_CallsRepositoryOnce()
        {
            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Users());

            await this.sut.GetUserByIdAsync(1);

            this.userRepositoryMock.Verify(
                r => r.GetByIdAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenRepositoryThrows_PropagatesException()
        {
            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ThrowsAsync(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.GetUserByIdAsync(1));
        }
    }
}