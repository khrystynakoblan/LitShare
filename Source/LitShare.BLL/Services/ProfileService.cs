namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class ProfileService : IProfileService
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<ProfileService> logger;

        public ProfileService(
            IUserRepository userRepository,
            ILogger<ProfileService> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
        }

        public async Task<Result<Users>> GetUserByIdAsync(int id)
        {
            this.logger.LogInformation("Fetching user profile. UserId: {UserId}", id);
            var user = await this.userRepository.GetByIdAsync(id);

            if (user == null)
            {
                this.logger.LogWarning("User not found. UserId: {UserId}", id);
                return Result<Users>.Failure("Користувача не знайдено.");
            }

            this.logger.LogInformation("User profile loaded successfully. UserId: {UserId}", id);
            return Result<Users>.Success(user);
        }

        public async Task<Result<bool>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            this.logger.LogInformation("Updating profile. UserId: {UserId}", userId);

            var user = await this.userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                this.logger.LogWarning("User not found. UserId: {UserId}", userId);
                return Result<bool>.Failure("Користувача не знайдено.");
            }

            user.Email = dto.Email;
            user.Region = dto.Region;
            user.Phone = dto.Phone;
            user.District = dto.District;
            user.City = dto.City;
            user.About = dto.About;
            user.PhotoUrl = dto.PhotoUrl;

            await this.userRepository.UpdateAsync(user);

            this.logger.LogInformation("Profile updated successfully. UserId: {UserId}", userId);

            return Result<bool>.Success(true);
        }
    }
}