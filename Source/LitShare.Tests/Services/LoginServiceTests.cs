namespace LitShare.Tests.Services
{
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
        public async Task LoginAsync_WithValidCredentials_ReturnsTrue()
        {
            var dto = ValidDto();
            this.SetupUserFound(dto.Email);
            this.SetupPasswordValid(dto.Password);

            bool result = await this.sut.LoginAsync(dto);

            Assert.True(result);
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
        public async Task LoginAsync_WhenUserNotFound_ReturnsFalse()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Users?)null);

            bool result = await this.sut.LoginAsync(dto);

            Assert.False(result);
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
        public async Task LoginAsync_WhenPasswordIsWrong_ReturnsFalse()
        {
            var dto = ValidDto();
            this.SetupUserFound(dto.Email);
            this.passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    dto.Password))
                .Returns(PasswordVerificationResult.Failed);

            bool result = await this.sut.LoginAsync(dto);

            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_WhenUserHasNoPasswordHash_ReturnsFalse()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new Users
                {
                    Email = dto.Email,
                    PasswordHash = string.Empty,
                });

            bool result = await this.sut.LoginAsync(dto);

            Assert.False(result);
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
        public async Task LoginAsync_WithNullDto_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => this.sut.LoginAsync(null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task LoginAsync_WithEmptyEmail_ThrowsArgumentException(string emptyEmail)
        {
            var dto = ValidDto();
            dto.Email = emptyEmail;

            await Assert.ThrowsAsync<ArgumentException>(
                () => this.sut.LoginAsync(dto));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task LoginAsync_WithEmptyPassword_ThrowsArgumentException(string emptyPassword)
        {
            var dto = ValidDto();
            dto.Password = emptyPassword;

            await Assert.ThrowsAsync<ArgumentException>(
                () => this.sut.LoginAsync(dto));
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
        public async Task LoginAsync_WithRehashNeeded_ReturnsTrue()
        {
            var dto = ValidDto();
            this.SetupUserFound(dto.Email);
            this.passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(
                    It.IsAny<Users>(),
                    It.IsAny<string>(),
                    dto.Password))
                .Returns(PasswordVerificationResult.SuccessRehashNeeded);

            bool result = await this.sut.LoginAsync(dto);

            Assert.True(result);
        }

        private static LoginDto ValidDto() => new LoginDto
        {
            Email = "ivan123@gmail.com",
            Password = "SecurePass123!",
        };

        private void SetupUserFound(string email)
        {
            this.userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(new Users
                {
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