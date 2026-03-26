using LitShare.BLL.DTOs;
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
            userRepositoryMock = new Mock<IUserRepository>();
            loggerMock = new Mock<ILogger<ProfileService>>();

            sut = new ProfileService(
                userRepositoryMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
        {
            var user = new Users { Id = 1, Name = "Test" };

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var result = await sut.GetUserByIdAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Test", result.Value.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            userRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Users?)null);

            var result = await sut.GetUserByIdAsync(1);

            Assert.False(result.IsSuccess);
            Assert.Equal("Користувача не знайдено.", result.Error);
        }

        [Fact]
        public async Task GetUserByIdAsync_CallsRepositoryOnce()
        {
            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Users());

            await sut.GetUserByIdAsync(1);

            userRepositoryMock.Verify(
                r => r.GetByIdAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenRepositoryThrows_PropagatesException()
        {
            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ThrowsAsync(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.GetUserByIdAsync(1));
        }

        [Fact]
        public async Task UpdateProfileAsync_WhenUserExists_UpdatesAllFields()
        {
            var user = new Users { Id = 1 };

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var dto = new UpdateProfileDto
            {
                Email = "new@test.com",
                Phone = "099",
                City = "Lviv",
                District = "District",
                Region = "Region",
                About = "About"
            };

            var result = await sut.UpdateProfileAsync(1, dto);

            Assert.True(result.IsSuccess);

            Assert.Equal(dto.Email, user.Email);
            Assert.Equal(dto.Phone, user.Phone);
            Assert.Equal(dto.City, user.City);
            Assert.Equal(dto.District, user.District);
            Assert.Equal(dto.Region, user.Region);
            Assert.Equal(dto.About, user.About);

            userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileAsync_WhenUserNotFound_ReturnsFailure()
        {
            userRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Users?)null);

            var result = await sut.UpdateProfileAsync(1, new UpdateProfileDto());

            Assert.False(result.IsSuccess);
            Assert.Equal("Користувача не знайдено.", result.Error);
        }

        [Fact]
        public async Task UpdateProfileAsync_WhenUserNotFound_DoesNotCallUpdate()
        {
            userRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Users?)null);

            await sut.UpdateProfileAsync(1, new UpdateProfileDto());

            userRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Users>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateProfileAsync_WhenRepositoryThrows_PropagatesException()
        {
            var user = new Users { Id = 1 };

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            userRepositoryMock
                .Setup(r => r.UpdateAsync(user))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(
                () => sut.UpdateProfileAsync(1, new UpdateProfileDto()));
        }

        [Fact]
        public async Task GenerateRandomAvatarAsync_WhenUserExists_UpdatesPhotoUrl()
        {
            var user = new Users { Id = 1, PhotoUrl = "old" };

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var result = await sut.GenerateRandomAvatarAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(user.PhotoUrl);
            Assert.NotEqual("old", user.PhotoUrl);
            Assert.Contains("dicebear", user.PhotoUrl);

            userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task GenerateRandomAvatarAsync_WhenUserNotFound_ReturnsFailure()
        {
            userRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Users?)null);

            var result = await sut.GenerateRandomAvatarAsync(1);

            Assert.False(result.IsSuccess);

            userRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Users>()),
                Times.Never);
        }

        [Fact]
        public async Task GenerateRandomAvatarAsync_WhenRepositoryThrows_PropagatesException()
        {
            var user = new Users { Id = 1 };

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            userRepositoryMock
                .Setup(r => r.UpdateAsync(user))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(
                () => sut.GenerateRandomAvatarAsync(1));
        }
    }
}