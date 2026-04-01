namespace LitShare.Web.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc;

    public abstract class BaseController : Controller
    {
        protected int GetCurrentUserId()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId!);
        }

        protected IActionResult HandleFailure(string error)
        {
            return this.Content(error);
        }
    }
}