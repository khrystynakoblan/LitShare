namespace LitShare.Web.Controllers
{
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
    }
}