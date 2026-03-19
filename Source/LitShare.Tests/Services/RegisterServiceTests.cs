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
        public async Task RegisterAsync_WithValidDto_ReturnsTrue()
        {
            var dto = ValidDto();
            this.SetupEmailFree(dto.Email);
            this.SetupHasher(dto.Password);

            bool result = await this.sut.RegisterAsync(dto);

            Assert.True(result);
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
        public async Task RegisterAsync_WithNullOptionalFields_ReturnsTrue()
        {
            var dto = ValidDto();
            dto.Phone = null;
            dto.Region = null;
            dto.District = null;
            dto.City = null;
            this.SetupEmailFree(dto.Email);
            this.SetupHasher(dto.Password);

            bool result = await this.sut.RegisterAsync(dto);

            Assert.True(result);
        }

        // ---------------------------------------------------------------
        // НЕГАТИВНІ СЦЕНАРІЇ
        // ---------------------------------------------------------------

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyTaken_ReturnsFalse()
        {
            var dto = ValidDto();
            this.userRepositoryMock
                .Setup(r => r.ExistsByEmailAsync(dto.Email))
                .ReturnsAsync(true);

            bool result = await this.sut.RegisterAsync(dto);

            Assert.False(result);
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
        public async Task RegisterAsync_WithNullDto_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => this.sut.RegisterAsync(null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithEmptyEmail_ThrowsArgumentException(string emptyEmail)
        {
            var dto = ValidDto();
            dto.Email = emptyEmail;

            await Assert.ThrowsAsync<ArgumentException>(
                () => this.sut.RegisterAsync(dto));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithEmptyPassword_ThrowsArgumentException(string emptyPassword)
        {
            var dto = ValidDto();
            dto.Password = emptyPassword;

            await Assert.ThrowsAsync<ArgumentException>(
                () => this.sut.RegisterAsync(dto));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithEmptyName_ThrowsArgumentException(string emptyName)
        {
            
            var dto = ValidDto();
            dto.Name = emptyName;

            
            await Assert.ThrowsAsync<ArgumentException>(
                () => this.sut.RegisterAsync(dto));
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