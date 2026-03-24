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

    public class RegisterServiceTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IPasswordHasher<Users>> passwordHasherMock;
        private readonly Mock<ILogger<RegisterService>> loggerMock;
        private readonly RegisterService sut;

        public RegisterServiceTests()
        {
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.passwordHasherMock = new Mock<IPasswordHasher<Users>>();
            this.loggerMock = new Mock<ILogger<RegisterService>>();

            this.sut = new RegisterService(
                this.userRepositoryMock.Object,
                this.passwordHasherMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithValidDto_ReturnsSuccess()
        {
            var dto = ValidDto();
            this.SetupEmailFree(dto.Email);
            this.SetupHasher(dto.Password);

            var result = await this.sut.RegisterAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value); 
        }

        [Fact]
        public async Task RegisterAsync_WithValidDto_CallsAddAsyncOnce()
        {
            var dto = ValidDto();
            this.SetupEmailFree(dto.Email);
            this.SetupHasher(dto.Password);

            await this.sut.RegisterAsync(dto);

            this.userRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Users>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithValidDto_HashesPasswordBeforeSaving()
        {
            var dto = ValidDto();
            this.SetupEmailFree(dto.Email);
            this.SetupHasher(dto.Password);

            await this.sut.RegisterAsync(dto);

            this.passwordHasherMock.Verify(
                h => h.HashPassword(It.IsAny<Users>(), dto.Password),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithValidDto_SavesUserWithRoleUser()
        {
            var dto = ValidDto();
            this.SetupEmailFree(dto.Email);
            this.SetupHasher(dto.Password);

            Users? savedUser = null;
            this.userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Users>()))
                .Callback<Users>(u => savedUser = u)
                .Returns(Task.CompletedTask);

            await this.sut.RegisterAsync(dto);

            Assert.NotNull(savedUser);
            Assert.Equal(RoleType.User, savedUser!.Role);
        }

        [Fact]
        public async Task RegisterAsync_WithNullOptionalFields_ReturnsSuccess()
        {
            var dto = ValidDto();
            dto.Phone = null;
            dto.Region = null;
            dto.District = null;
            dto.City = null;
            this.SetupEmailFree(dto.Email);
            this.SetupHasher(dto.Password);

            var result = await this.sut.RegisterAsync(dto);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyTaken_ReturnsFailure()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.ExistsByEmailAsync(dto.Email))
                .ReturnsAsync(true);

            var result = await this.sut.RegisterAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Цей email вже зареєстрований у системі.", result.Error);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyTaken_NeverCallsAddAsync()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.ExistsByEmailAsync(dto.Email))
                .ReturnsAsync(true);

            await this.sut.RegisterAsync(dto);

            this.userRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Users>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WithNullDto_ReturnsFailure()
        {
            var result = await this.sut.RegisterAsync(null!);
            Assert.False(result.IsSuccess);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithEmptyEmail_ReturnsFailure(string emptyEmail)
        {
            var dto = ValidDto();
            dto.Email = emptyEmail;

            var result = await this.sut.RegisterAsync(dto);
            Assert.False(result.IsSuccess);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithEmptyPassword_ReturnsFailure(string emptyPassword)
        {
            var dto = ValidDto();
            dto.Password = emptyPassword;

            var result = await this.sut.RegisterAsync(dto);
            Assert.False(result.IsSuccess);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithEmptyName_ReturnsFailure(string emptyName)
        {
            var dto = ValidDto();
            dto.Name = emptyName;

            var result = await this.sut.RegisterAsync(dto);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RegisterAsync_WhenRepositoryThrows_PropagatesException()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.ExistsByEmailAsync(dto.Email))
                .ThrowsAsync(new InvalidOperationException("DB connection lost"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.RegisterAsync(dto));
        }

        private static RegisterDto ValidDto() => new RegisterDto
        {
            Name = "Іван",
            Email = "ivan123@gmail.com",
            Password = "SecurePass123!",
            Phone = "+380991234567",
            Region = "Львівська",
            District = "Личаківський",
            City = "Львів",
        };

        private void SetupEmailFree(string email)
        {
            this.userRepositoryMock
                .Setup(r => r.ExistsByEmailAsync(email))
                .ReturnsAsync(false);
        }

        private void SetupHasher(string password)
        {
            this.passwordHasherMock
                .Setup(h => h.HashPassword(It.IsAny<Users>(), password))
                .Returns("hashed_password_value");

            this.userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Users>()))
                .Returns(Task.CompletedTask);
        }
    }
}