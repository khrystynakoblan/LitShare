namespace LitShare.BLL.Services
{
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher<Users> passwordHasher;
        private readonly ILogger<RegisterService> logger;

        public RegisterService(
            IUserRepository userRepository,
            IPasswordHasher<Users> passwordHasher,
            ILogger<RegisterService> logger)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.logger = logger;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Ім'я не може бути порожнім.", nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email не може бути порожнім.", nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Пароль не може бути порожнім.", nameof(dto));
            }

            this.logger.LogInformation(
                "Registration attempt. Email: {Email}", dto.Email);

            bool emailTaken = await this.userRepository.ExistsByEmailAsync(dto.Email);

            if (emailTaken)
            {
                this.logger.LogWarning(
                    "Registration rejected: email {Email} is already taken.", dto.Email);
                return false;
            }

            var user = new Users
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Region = dto.Region,
                District = dto.District,
                City = dto.City,
                Role = RoleType.User,
            };

            user.PasswordHash = this.passwordHasher.HashPassword(user, dto.Password);

            await this.userRepository.AddAsync(user);

            this.logger.LogInformation(
                "User registered successfully. Email: {Email}", dto.Email);

            return true;
        }
    }
}