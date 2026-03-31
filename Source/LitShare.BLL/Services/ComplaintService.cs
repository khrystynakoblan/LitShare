namespace LitShare.BLL.Services
{
    using System;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository complaintRepository;
        private readonly ILogger<ComplaintService> logger;

        public ComplaintService(
            IComplaintRepository complaintRepository,
            ILogger<ComplaintService> logger)
        {
            this.complaintRepository = complaintRepository;
            this.logger = logger;
        }

        public async Task<Result<bool>> CreateComplaintAsync(CreateComplaintDto dto, int userId)
        {
            this.logger.LogInformation("User {UserId} creating complaint for post {PostId}. Type: {Type}", userId, dto.PostId, dto.ComplaintType);
            var text = string.IsNullOrWhiteSpace(dto.AdditionalText)
                ? dto.ComplaintType
                : $"[{dto.ComplaintType}] {dto.AdditionalText}";

            var complaint = new Complaints
            {
                PostId = dto.PostId,
                ComplainantId = userId,
                Text = text,
                Date = DateTime.UtcNow,
            };

            await this.complaintRepository.AddAsync(complaint);
            await this.complaintRepository.SaveChangesAsync();

            this.logger.LogInformation("Complaint successfully created for post {PostId} by user {UserId}", dto.PostId, userId);
            return Result<bool>.Success(true);
        }
    }
}