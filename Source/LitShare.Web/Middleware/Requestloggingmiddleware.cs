namespace LitShare.Web.Middleware
{
    using System.Security.Claims;
    using System.Text;
    using System.Web;

    public class RequestLoggingMiddleware
    {
        private static readonly HashSet<string> SensitiveFields = new (StringComparer.OrdinalIgnoreCase)
        {
            "Password",
            "ConfirmPassword",
            "__RequestVerificationToken",
        };

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
                var rawBody = Encoding.UTF8.GetString(memoryStream.ToArray());
                request.Body.Position = 0;

                requestBody = this.RedactSensitiveFields(rawBody, request.ContentType);
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

        private string RedactSensitiveFields(string rawBody, string? contentType)
        {
            if (contentType != null && contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                return "[multipart form data — not logged]";
            }

            if (contentType != null && contentType.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                var pairs = rawBody.Split('&');
                var redacted = pairs.Select(pair =>
                {
                    var eqIndex = pair.IndexOf('=');
                    if (eqIndex < 0)
                    {
                        return pair;
                    }

                    var key = HttpUtility.UrlDecode(pair[..eqIndex]);
                    var value = HttpUtility.UrlDecode(pair[(eqIndex + 1) ..]);

                    return SensitiveFields.Contains(key)
                        ? $"{key}=[REDACTED]"
                        : $"{key}={value}";
                });

                return string.Join(" & ", redacted);
            }

            return rawBody;
        }
    }
}