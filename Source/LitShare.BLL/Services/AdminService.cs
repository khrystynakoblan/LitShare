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
        private readonly ILogger<AdminService> logger;

        public AdminService(IComplaintRepository complaintRepository, ILogger<AdminService> logger)
        {
            this.complaintRepository = complaintRepository;
            this.logger = logger;
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
    }
}