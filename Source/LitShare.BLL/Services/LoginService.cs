namespace LitShare.BLL.Services
{
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

        public async Task<bool> LoginAsync(LoginDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email не може бути порожнім.", nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Пароль не може бути порожнім.", nameof(dto));
            }

            this.logger.LogInformation("Login attempt. Email: {Email}", dto.Email);

            var user = await this.userRepository.GetByEmailAsync(dto.Email);

            if (user is null)
            {
                this.logger.LogWarning("Login failed: user not found. Email: {Email}", dto.Email);
                return false;
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                this.logger.LogWarning("Login failed: user has no password hash. Email: {Email}", dto.Email);
                return false;
            }

            var result = this.passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                this.logger.LogWarning("Login failed: wrong password. Email: {Email}", dto.Email);
                return false;
            }

            this.logger.LogInformation("Login successful. Email: {Email}", dto.Email);
            return true;
        }
    }
}