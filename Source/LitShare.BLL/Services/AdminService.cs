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
        private readonly ILogger<AdminService> logger;

        public AdminService(
            IComplaintRepository complaintRepository,
            IPostRepository postRepository,
            ILogger<AdminService> logger)
        {
            this.complaintRepository = complaintRepository;
            this.postRepository = postRepository;
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
                int postId = complaint.Post.Id;

                await this.postRepository.DeleteAsync(complaint.Post);

                this.logger.LogInformation("Post {PostId} marked for deletion due to complaint {ComplaintId}", postId, id);
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

            this.logger.LogInformation("Fetched {Count} complaints.", dtos.Count);

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
    }
}