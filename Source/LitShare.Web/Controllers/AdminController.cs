namespace LitShare.Web.Controllers
{
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService adminService;
        private readonly IProfileService profileService;
        private readonly ILogger<AdminController> logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger, IProfileService profileService)
        {
            this.adminService = adminService;
            this.logger = logger;
            this.profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            this.logger.LogInformation("Admin navigated to complaints list.");

            var result = await this.adminService.GetAllComplaintsAsync();

            if (result.IsFailure)
            {
                this.logger.LogWarning("Failed to load complaints: {Error}", result.Error);
                return this.View(new AdminViewModel());
            }

            var model = new AdminViewModel
            {
                Complaints = result.Value!,
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            this.logger.LogInformation("Admin viewing complaint details. Id: {Id}", id);

            var result = await this.adminService.GetComplaintByIdAsync(id);

            if (result.IsFailure)
            {
                this.logger.LogWarning("Complaint not found. Id: {Id}", id);
                return this.RedirectToAction(nameof(this.Index));
            }

            var model = new ComplaintDetailsViewModel
            {
                Complaint = result.Value!,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            this.logger.LogInformation("Admin approving complaint. Id: {Id}", id);

            var result = await this.adminService.ApproveComplaintAsync(id);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return this.RedirectToAction(nameof(this.Details), new { id });
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            this.logger.LogInformation("Admin rejecting complaint. Id: {Id}", id);

            var result = await this.adminService.RejectComplaintAsync(id);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return this.RedirectToAction(nameof(this.Details), new { id });
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            this.logger.LogInformation("Admin viewing statistics page");

            var result = await this.adminService.GetStatisticsAsync();

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return this.View(new AdminStatsViewModel());
            }

            var model = new AdminStatsViewModel
            {
                Stats = result.Value!
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var userId = this.GetCurrentUserId();
            this.logger.LogInformation("Admin opening their profile. UserId: {UserId}", userId);

            var result = await this.profileService.GetUserByIdAsync(userId);

            if (result.IsFailure)
            {
                return this.HandleFailure(result.Error);
            }

            var user = result.Value!;

            var model = new ProfileViewModel
            {
                UserId = userId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone ?? string.Empty,
                PhotoUrl = user.PhotoUrl,
                About = user.About ?? "????????????? ????????? LitShare",
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            this.logger.LogInformation("Admin viewing users list.");

            var result = await this.adminService.GetAllUsersAsync();

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBlockUser(int id)
        {
            this.logger.LogInformation("Admin requested to toggle block status for user {Id}", id);

            var result = await this.adminService.ToggleUserBlockAsync(id);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return this.RedirectToAction(nameof(this.Users));
        }
    }
}