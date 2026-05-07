namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class AdminService : IAdminService
    {
        private readonly IComplaintRepository complaintRepository;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly ILogger<AdminService> logger;
        private readonly INotificationRepository notificationRepository;
        private readonly AppSettings settings;

        public AdminService(
            IComplaintRepository complaintRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            ILogger<AdminService> logger,
            INotificationRepository notificationRepository,
            IOptions<AppSettings> options)
        {
            this.complaintRepository = complaintRepository;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.notificationRepository = notificationRepository;
            this.settings = options.Value;
        }

        public async Task<Result<bool>> ApproveComplaintAsync(int id)
        {
            this.logger.LogInformation("Approving complaint. Id: {Id}", id);

            var complaint = await this.complaintRepository.GetByIdAsync(id);

            if (complaint == null)
            {
                return Result<bool>.Failure("Скаргу не знайдено.");
            }

            if (complaint.Post != null)
            {
                int ownerId = complaint.Post.UserId;
                string bookTitle = complaint.Post.Title ?? "Без назви";

                var notification = new Notifications 
                {
                    UserId = ownerId,
                    Message = $"Ваше оголошення '{bookTitle}' було видалено модератором через скаргу.",
                    IsSent = false
                };

                await this.notificationRepository.AddAsync(notification);
                await this.notificationRepository.SaveChangesAsync();

                await this.postRepository.DeletePostAsync(complaint.Post);
                await this.postRepository.SaveChangesAsync();
            }

            var checkComplaint = await this.complaintRepository.GetByIdAsync(id);
            if (checkComplaint != null)
            {
                await this.complaintRepository.DeleteAsync(checkComplaint);
                await this.complaintRepository.SaveChangesAsync();
            }

            return true;
        }

        public async Task<Result<List<ComplaintDto>>> GetAllComplaintsAsync()
        {
            this.logger.LogInformation("Fetching all complaints for admin.");

            var complaints = await this.complaintRepository.GetAllAsync();

            var dtos = complaints.Select(c => new ComplaintDto
            {
                Id = c.Id,
                Text = c.Text ?? string.Empty,
                BookTitle = c.Post?.Title ?? string.Empty,
                ComplainantName = c.Complainant?.Name ?? string.Empty,
                Date = c.Date,
            }).ToList();

            return dtos;
        }

        public async Task<Result<ComplaintDetailsDto>> GetComplaintByIdAsync(int id)
        {
            this.logger.LogInformation("Fetching complaint details. Id: {Id}", id);

            var complaint = await this.complaintRepository.GetByIdAsync(id);

            if (complaint == null)
            {
                return Result<ComplaintDetailsDto>.Failure("Скаргу не знайдено.");
            }

            var dto = new ComplaintDetailsDto
            {
                Id = complaint.Id,
                Text = complaint.Text ?? string.Empty,
                ComplainantName = complaint.Complainant?.Name ?? string.Empty,
                Date = complaint.Date,
                BookTitle = complaint.Post?.Title ?? string.Empty,
                BookAuthor = complaint.Post?.Author ?? string.Empty,
                BookDescription = complaint.Post?.Description ?? string.Empty,
                BookPhotoUrl = complaint.Post?.PhotoUrl,
            };

            return dto;
        }

        public async Task<Result<bool>> RejectComplaintAsync(int id)
        {
            this.logger.LogInformation("Rejecting complaint. Id: {Id}", id);

            var complaint = await this.complaintRepository.GetByIdAsync(id);

            if (complaint == null)
            {
                return Result<bool>.Failure("Скаргу не знайдено.");
            }

            await this.complaintRepository.DeleteAsync(complaint);
            await this.complaintRepository.SaveChangesAsync();

            return true;
        }

        public async Task<Result<AdminStatsDto>> GetStatisticsAsync()
        {
            this.logger.LogInformation("Fetching admin statistics");

            var users = await this.userRepository.GetAllAsync();
            var posts = await this.postRepository.GetAllAsync();
            var complaints = await this.complaintRepository.GetAllAsync();

            var cityStats = posts
                .Where(p => p.User?.City != null)
                .GroupBy(p => p.User!.City!)
                .Select(g => new CityStatDto { City = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(this.settings.AdminStatsTopCitiesCount)
                .ToList();

            var genreStats = posts
                .Where(p => p.BookGenres != null && p.BookGenres.Any())
                .SelectMany(p => p.BookGenres!)
                .Where(bg => bg.Genre?.Name != null)
                .GroupBy(bg => bg.Genre!.Name!)
                .Select(g => new GenreStatDto
                {
                    GenreName = g.Key ?? "Без жанру",
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(this.settings.AdminStatsTopGenresCount)
                .ToList();

            var stats = new AdminStatsDto
            {
                TotalUsers = users.Count(),
                TotalPosts = posts.Count(),
                ActivePosts = posts.Count(p => p.IsActive),
                TotalComplaints = complaints.Count(),
                PendingComplaints = complaints.Count(),
                TopCities = cityStats,
                TopGenres = genreStats
            };

            return stats;
        }

        public async Task<Result<IEnumerable<AdminUserListDto>>> GetAllUsersAsync()
        {
            this.logger.LogInformation("Fetching all users for admin panel.");

            var users = await this.userRepository.GetAllAsync();

            var dtos = users
                        .OrderBy(u => u.Id)
                        .Select(u => new AdminUserListDto
                        {
                            Id = u.Id,
                            Name = u.Name ?? "Без імені",
                            Email = u.Email ?? "Не вказано",
                            Phone = u.Phone ?? "-",
                            Location = $"{u.City}, {u.Region}",
                            Role = u.Role.ToString(),
                            IsBlocked = u.IsBlocked
                        });

            return Result<IEnumerable<AdminUserListDto>>.Success(dtos);
        }

        public async Task<Result<bool>> ToggleUserBlockAsync(int userId)
        {
            this.logger.LogInformation("Toggling block status for user. UserId: {UserId}", userId);

            var user = await this.userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result<bool>.Failure("Користувача не знайдено.");
            }

            if (user.Role == RoleType.Admin)
            {
                return Result<bool>.Failure("Неможливо заблокувати адміністратора.");
            }

            user.IsBlocked = !user.IsBlocked;

            await this.userRepository.UpdateAsync(user);
            this.logger.LogInformation("User {UserId} block status changed to {IsBlocked}", userId, user.IsBlocked);

            return true;
        }
    }
}