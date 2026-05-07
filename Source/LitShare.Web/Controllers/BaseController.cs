using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace LitShare.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated properly");
            }

            return userId;
        }

        protected IActionResult HandleFailure(string error)
        {
            return Content(error);
        }
    }
}