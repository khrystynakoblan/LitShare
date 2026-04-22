namespace LitShare.Web.Middleware
{
    using System.Security.Claims;
    using System.Text;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestLoggingMiddleware> logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            string requestBody = string.Empty;
            if (request.Method is "POST" or "PUT" or "PATCH")
            {
                request.EnableBuffering();
                using var memoryStream = new MemoryStream();
                await request.Body.CopyToAsync(memoryStream);
                requestBody = Encoding.UTF8.GetString(memoryStream.ToArray());
                request.Body.Position = 0;

                if (requestBody.Contains("Password") ||
                    requestBody.Contains("__RequestVerificationToken"))
                {
                    requestBody = "[sensitive data hidden]";
                }
            }

            var headers = request.Headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
                         && !h.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

            this.logger.LogInformation(
                "Incoming request | Method: {Method} | URL: {Url} | IP: {Ip} | UserId: {UserId} | Headers: {@Headers} | Body: {Body}",
                request.Method,
                $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}",
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                userId,
                headers,
                string.IsNullOrWhiteSpace(requestBody) ? "(empty)" : requestBody);

            await this.next(context);
        }
    }
}