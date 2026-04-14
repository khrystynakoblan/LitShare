namespace LitShare.Web.Controllers
{
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService adminService;
        private readonly ILogger<AdminController> logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            this.adminService = adminService;
            this.logger = logger;
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

            TempData["SuccessMessage"] = "Скаргу відхилено.";
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
    }
}