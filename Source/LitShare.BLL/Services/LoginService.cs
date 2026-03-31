namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class LoginService : ILoginService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher<Users> passwordHasher;
        private readonly ILogger<LoginService> logger;

        public LoginService(
            IUserRepository userRepository,
            IPasswordHasher<Users> passwordHasher,
            ILogger<LoginService> logger)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.logger = logger;
        }

        public async Task<Result<LoginResultDto>> LoginAsync(LoginDto dto)
        {
            if (dto == null)
            {
                return Result<LoginResultDto>.Failure("Дані порожні.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Result<LoginResultDto>.Failure("Email та пароль не можуть бути порожніми.");
            }

            this.logger.LogInformation("Login attempt. Email: {Email}", dto.Email);

            var user = await this.userRepository.GetByEmailAsync(dto.Email);

            if (user is null || string.IsNullOrEmpty(user.PasswordHash))
            {
                this.logger.LogWarning("Login failed. Email: {Email}", dto.Email);
                return Result<LoginResultDto>.Failure("Невірний email або пароль.");
            }

            var result = this.passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                this.logger.LogWarning("Login failed: wrong password. Email: {Email}", dto.Email);
                return Result<LoginResultDto>.Failure("Невірний email або пароль.");
            }

            this.logger.LogInformation("Login successful. Email: {Email}, Role: {Role}", dto.Email, user.Role);

            return Result<LoginResultDto>.Success(new LoginResultDto
            {
                UserId = user.Id,
                Role = user.Role,
            });
        }
    }
}