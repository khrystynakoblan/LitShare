namespace LitShare.Web.Controllers
{
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    public class ComplaintController : BaseController
    {
        private readonly IComplaintService complaintService;
        private readonly ILogger<ComplaintController> logger;

        public ComplaintController(
            IComplaintService complaintService,
            ILogger<ComplaintController> logger)
        {
            this.complaintService = complaintService;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Create(int postId)
        {
            this.logger.LogInformation("Navigating to complaint form for post ID: {PostId}", postId);
            var model = new ComplaintViewModel { PostId = postId };
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            int currentUserId = this.GetCurrentUserId();
            this.logger.LogInformation("User {UserId} submitting complaint for post ID: {PostId}", currentUserId, model.PostId);

            var dto = new CreateComplaintDto
            {
                PostId = model.PostId,
                ComplaintType = model.ComplaintType,
                AdditionalText = model.AdditionalText,
            };

            var result = await this.complaintService.CreateComplaintAsync(dto, currentUserId);

            if (result.IsFailure)
            {
                this.ModelState.AddModelError(string.Empty, result.Error);
                return this.View(model);
            }

            this.TempData["SuccessMessage"] = "Скаргу успішно надіслано.";
            return this.RedirectToAction("Details", "Post", new { id = model.PostId });
        }
    }
}