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
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
        {
            var user = new Users { Id = 1, Name = "Test" };

            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var result = await this.sut.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test", result!.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Users?)null);

            var result = await this.sut.GetUserByIdAsync(1);

            Assert.Null(result);
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