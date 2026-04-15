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

        public ExchangeController(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(int postId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized();
            }

            var result = await this.exchangeService.CreateRequestAsync(postId, currentUserId);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Запит надіслано";
            }
            else
            {
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
                return Unauthorized();
            }

            var requests = await this.exchangeService.GetSentRequestsAsync(currentUserId);
            return View(requests);
        }
    }
}