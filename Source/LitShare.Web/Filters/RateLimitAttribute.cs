namespace LitShare.Web.Filters
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RateLimitAttribute : ActionFilterAttribute
    {
        public int MaxRequests { get; set; } = 10;

        public int WindowSeconds { get; set; } = 60;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<RateLimitAttribute>>();

            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var endpoint = $"{context.RouteData.Values["controller"]}:{context.RouteData.Values["action"]}";
            var cacheKey = $"ratelimit:{ip}:{endpoint}";

            var requestCount = cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(this.WindowSeconds);
                return 0;
            });

            requestCount++;
            cache.Set(cacheKey, requestCount, TimeSpan.FromSeconds(this.WindowSeconds));

            logger.LogInformation(
                "RateLimit check — IP: {IP}, Endpoint: {Endpoint}, Count: {Count}/{Max}",
                ip,
                endpoint,
                requestCount,
                this.MaxRequests);

            if (requestCount > this.MaxRequests)
            {
                logger.LogWarning(
                    "Rate limit exceeded — IP: {IP}, Endpoint: {Endpoint}, Count: {Count}",
                    ip,
                    endpoint,
                    requestCount);

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Result = new RedirectToActionResult("TooManyRequests", "Home", null);
            }
        }
    }
}