namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class AdminService : IAdminService
    {
        private readonly IComplaintRepository complaintRepository;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly ILogger<AdminService> logger;

        public AdminService(
            IComplaintRepository complaintRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            ILogger<AdminService> logger)
        {
            this.complaintRepository = complaintRepository;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.logger = logger;
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
                await this.postRepository.DeletePostAsync(complaint.Post);
                await this.postRepository.SaveChangesAsync();
            }

            await this.complaintRepository.DeleteAsync(complaint);
            await this.complaintRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
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

            return Result<List<ComplaintDto>>.Success(dtos);
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

            return Result<ComplaintDetailsDto>.Success(dto);
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

            return Result<bool>.Success(true);
        }

        public async Task<Result<AdminStatsDto>> GetStatisticsAsync()
        {
            this.logger.LogInformation("Fetching admin statistics");

            try
            {
                var users = await this.userRepository.GetAllAsync();
                var posts = await this.postRepository.GetAllPostsAsync();
                var complaints = await this.complaintRepository.GetAllAsync();

                var cityStats = posts
                    .Where(p => p.User?.City != null)
                    .GroupBy(p => p.User!.City!)
                    .Select(g => new CityStatDto
                    {
                        City = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                var genreStats = posts
                    .Where(p => p.BookGenres != null)
                    .SelectMany(p => p.BookGenres!)
                    .Where(bg => bg.Genre?.Name != null)
                    .GroupBy(bg => bg.Genre!.Name!)
                    .Select(g => new GenreStatDto
                    {
                        GenreName = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                var stats = new AdminStatsDto
                {
                    TotalUsers = users.Count(),
                    TotalPosts = posts.Count(),
                    ActivePosts = posts.Count(),
                    TotalComplaints = complaints.Count(),
                    PendingComplaints = complaints.Count(),
                    TopCities = cityStats,
                    TopGenres = genreStats
                };

                return Result<AdminStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error fetching admin statistics");
                return Result<AdminStatsDto>.Failure("Не вдалося завантажити статистику");
            }
        }

        public Task GetStatisticsAsync(object key)
        {
            throw new NotImplementedException();
        }
    }
}