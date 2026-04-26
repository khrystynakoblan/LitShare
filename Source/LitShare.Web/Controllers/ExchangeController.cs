using System.Security.Claims;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LitShare.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class ExchangeController : BaseController
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

            var currentUserId = this.GetCurrentUserId();

            var result = await this.exchangeService.CreateRequestAsync(postId, currentUserId);

            if (result.IsSuccess)
            {
                this.logger.LogInformation(
                    "Successfully created request for PostId: {PostId} by UserId: {UserId}",
                    postId,
                    currentUserId);

                TempData["SuccessMessage"] = "Запит надіслано";
            }
            else
            {
                this.logger.LogWarning(
                    "Service returned failure for CreateRequest. PostId: {PostId}, UserId: {UserId}, Error: {Error}",
                    postId,
                    currentUserId,
                    result.Error);

                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("Details", "Post", new { id = postId });
        }

        [HttpGet]
        public async Task<IActionResult> SentRequests()
        {
            var currentUserId = this.GetCurrentUserId();

            this.logger.LogInformation("Fetching sent requests for UserId: {UserId}", currentUserId);

            var requests = await this.exchangeService.GetSentRequestsAsync(currentUserId);

            this.logger.LogInformation("Successfully retrieved sent requests for UserId: {UserId}", currentUserId);

            return this.View(requests);
        }

        [HttpGet]
        public async Task<IActionResult> ReceivedRequests()
        {
            var userId = this.GetCurrentUserId();

            this.logger.LogInformation("Fetching received requests for UserId: {UserId}", userId);

            var result = await this.exchangeService.GetReceivedRequestsAsync(userId);

            if (result.IsFailure)
            {
                this.logger.LogWarning(
                    "Failed to fetch received requests for UserId: {UserId}. Error: {Error}",
                    userId,
                    result.Error);

                return this.HandleFailure(result.Error);
            }

            this.logger.LogInformation("Successfully retrieved received requests for UserId: {UserId}", userId);

            return this.View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var currentUserId = this.GetCurrentUserId();

            var result = await this.exchangeService.UpdateRequestStatusAsync(requestId, currentUserId, RequestStatus.Accepted);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Запит успішно прийнято!";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("ReceivedRequests");
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var currentUserId = this.GetCurrentUserId();

            var result = await this.exchangeService.UpdateRequestStatusAsync(requestId, currentUserId, RequestStatus.Rejected);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Запит відхилено.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("ReceivedRequests");
        }

        [HttpPost]
        public async Task<IActionResult> CompleteDeal(int requestId)
        {
            var currentUserId = this.GetCurrentUserId();

            this.logger.LogInformation(
                "User {UserId} is attempting to complete deal for RequestId: {RequestId}",
                currentUserId,
                requestId);

            var result = await this.exchangeService.CompleteDealAsync(requestId, currentUserId);

            if (result.IsSuccess)
            {
                this.logger.LogInformation(
                    "Deal successfully completed. RequestId: {RequestId}",
                    requestId);

                TempData["SuccessMessage"] = "Угоду виконано";
            }
            else
            {
                this.logger.LogWarning(
                    "Failed to complete deal. RequestId: {RequestId}, Error: {Error}",
                    requestId,
                    result.Error);

                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("ReceivedRequests");
        }
    }
}