namespace LitShare.Web.Middleware
{
    using System.Security.Claims;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.Logging;

    public class BlockCheckMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<BlockCheckMiddleware> logger;

        public BlockCheckMiddleware(RequestDelegate next, ILogger<BlockCheckMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(userIdClaim, out int userId))
                {
                    var user = await userRepository.GetByIdAsync(userId);

                    if (user != null && user.IsBlocked)
                    {
                        this.logger.LogWarning("Blocked user {UserId} tried to navigate. Forcing logout.", userId);

                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        context.Response.Redirect("/Account/Login");

                        return;
                    }
                }
            }

            await this.next(context);
        }
    }
}