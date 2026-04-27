namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
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
        private readonly AppSettings settings;

        public AdminService(
            IComplaintRepository complaintRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            ILogger<AdminService> logger,
            IOptions<AppSettings> options)
        {
            this.complaintRepository = complaintRepository;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.logger = logger;
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

            try
            {
                if (complaint.Post != null)
                {
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
            catch (DbUpdateConcurrencyException)
            {
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Помилка при підтвердженні скарги");
                return Result<bool>.Failure("Виникла помилка при видаленні даних.");
            }
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

            try
            {
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
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error fetching admin statistics");
                return Result<AdminStatsDto>.Failure("Не вдалося завантажити статистику");
            }
        }

        public async Task<Result<IEnumerable<AdminUserListDto>>> GetAllUsersAsync()
        {
            this.logger.LogInformation("Fetching all users for admin panel.");

            try
            {
                var users = await this.userRepository.GetAllAsync();

                var dtos = users.Select(u => new AdminUserListDto
                {
                    Id = u.Id,
                    Name = u.Name ?? "Без імені",
                    Email = u.Email ?? "Не вказано",
                    Phone = u.Phone ?? "-",
                    Location = $"{u.City}, {u.Region}",
                    Role = u.Role.ToString()
                });

                return Result<IEnumerable<AdminUserListDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occurred while fetching users list.");
                return Result<IEnumerable<AdminUserListDto>>.Failure("Не вдалося завантажити список користувачів.");
            }
        }
    }
}