namespace LitShare.BLL.Services
{
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

        public async Task<Users?> GetUserByIdAsync(int id)
        {
            this.logger.LogInformation(
                "Fetching user profile. UserId: {UserId}",
                id);

            var user = await this.userRepository.GetByIdAsync(id);

            if (user == null)
            {
                this.logger.LogWarning(
                    "User not found. UserId: {UserId}",
                    id);

                return null;
            }

            this.logger.LogInformation(
                "User profile loaded successfully. UserId: {UserId}",
                id);

            return user;
        }
    }
}