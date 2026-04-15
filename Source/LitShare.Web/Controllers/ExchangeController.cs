using System.Security.Claims;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitShare.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class ExchangeController : Controller
    {
        private readonly IExchangeService exchangeService;
        private readonly ILogger<ExchangeController> logger;

        public ExchangeController(IExchangeService exchangeService, ILogger<ExchangeController> logger)
        {
            this.exchangeService = exchangeService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(int postId)
        {
            this.logger.LogInformation("User is attempting to create a request for PostId: {PostId}", postId);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int currentUserId))
            {
                this.logger.LogWarning("CreateRequest failed: Could not parse UserId from claims.");
                return Unauthorized();
            }

            var result = await this.exchangeService.CreateRequestAsync(postId, currentUserId);

            if (result.IsSuccess)
            {
                this.logger.LogInformation("Successfully created request for PostId: {PostId} by UserId: {UserId}", postId, currentUserId);
                TempData["SuccessMessage"] = "Запит надіслано";
            }
            else
            {
                this.logger.LogWarning("Service returned failure for CreateRequest. PostId: {PostId}, UserId: {UserId}, Error: {Error}", postId, currentUserId, result.Error);
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("Details", "Post", new { id = postId });
        }

        [HttpGet]
        public async Task<IActionResult> SentRequests()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int currentUserId))
            {
                this.logger.LogWarning("SentRequests access denied: Invalid UserId claim.");
                return Unauthorized();
            }

            this.logger.LogInformation("Fetching sent requests for UserId: {UserId}", currentUserId);

            var requests = await this.exchangeService.GetSentRequestsAsync(currentUserId);

            this.logger.LogInformation("Successfully retrieved sent requests for UserId: {UserId}", currentUserId);

            return View(requests);
        }
    }
}