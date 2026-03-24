namespace LitShare.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class LoginServiceTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IPasswordHasher<Users>> passwordHasherMock;
        private readonly Mock<ILogger<LoginService>> loggerMock;
        private readonly LoginService sut;

        public LoginServiceTests()
        {
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.passwordHasherMock = new Mock<IPasswordHasher<Users>>();
            this.loggerMock = new Mock<ILogger<LoginService>>();

            this.sut = new LoginService(
                this.userRepositoryMock.Object,
                this.passwordHasherMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccessWithUserId()
        {
            var dto = ValidDto();
            int expectedUserId = 5;
            this.SetupUserFound(dto.Email, expectedUserId);
            this.SetupPasswordValid(dto.Password);

            // БУЛО: bool result = ...
            // СТАЛО:
            var result = await this.sut.LoginAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedUserId, result.Value);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_VerifiesPasswordOnce()
        {
            var dto = ValidDto();
            this.SetupUserFound(dto.Email);
            this.SetupPasswordValid(dto.Password);

            await this.sut.LoginAsync(dto);

            this.passwordHasherMock.Verify(
                h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    dto.Password),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ReturnsFailure()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Users?)null);

            var result = await this.sut.LoginAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Невірний email або пароль.", result.Error);
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_NeverVerifiesPassword()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Users?)null);

            await this.sut.LoginAsync(dto);

            this.passwordHasherMock.Verify(
                h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsWrong_ReturnsFailure()
        {
            var dto = ValidDto();
            this.SetupUserFound(dto.Email);
            this.passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    dto.Password))
                .Returns(PasswordVerificationResult.Failed);

            var result = await this.sut.LoginAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Невірний email або пароль.", result.Error);
        }

        [Fact]
        public async Task LoginAsync_WhenUserHasNoPasswordHash_ReturnsFailure()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new Users
                {
                    Email = dto.Email,
                    PasswordHash = string.Empty,
                });

            var result = await this.sut.LoginAsync(dto);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task LoginAsync_WhenUserHasNoPasswordHash_NeverVerifiesPassword()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new Users
                {
                    Email = dto.Email,
                    PasswordHash = string.Empty,
                });

            await this.sut.LoginAsync(dto);

            this.passwordHasherMock.Verify(
                h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithNullDto_ReturnsFailure()
        {
            var result = await this.sut.LoginAsync(null!);
            Assert.False(result.IsSuccess);
            Assert.Equal("Дані порожні.", result.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task LoginAsync_WithEmptyEmail_ReturnsFailure(string emptyEmail)
        {
            var dto = ValidDto();
            dto.Email = emptyEmail;

            var result = await this.sut.LoginAsync(dto);
            Assert.False(result.IsSuccess);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task LoginAsync_WithEmptyPassword_ReturnsFailure(string emptyPassword)
        {
            var dto = ValidDto();
            dto.Password = emptyPassword;

            var result = await this.sut.LoginAsync(dto);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task LoginAsync_WhenRepositoryThrows_PropagatesException()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ThrowsAsync(new InvalidOperationException("DB connection lost"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_WithRehashNeeded_ReturnsSuccess()
        {
            var dto = ValidDto();
            this.SetupUserFound(dto.Email);
            this.passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    dto.Password))
                .Returns(PasswordVerificationResult.SuccessRehashNeeded);

            var result = await this.sut.LoginAsync(dto);

            Assert.True(result.IsSuccess);
        }

        private static LoginDto ValidDto() => new LoginDto
        {
            Email = "ivan123@gmail.com",
            Password = "SecurePass123!",
        };

        private void SetupUserFound(string email, int expectedId = 1)
        {
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(new Users
                {
                    Id = expectedId,
                    Email = email,
                    PasswordHash = "hashed_password_value",
                });
        }

        private void SetupPasswordValid(string password)
        {
            this.passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    password))
                .Returns(PasswordVerificationResult.Success);
        }
    }
}