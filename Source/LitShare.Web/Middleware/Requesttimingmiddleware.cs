namespace LitShare.Web.Middleware
{
    using System.Diagnostics;

    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestTimingMiddleware> logger;

        public RequestTimingMiddleware(
            RequestDelegate next,
            ILogger<RequestTimingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await this.next(context);

            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var method = context.Request.Method;
            var requestPath = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            if (elapsedMs > 1000)
            {
                this.logger.LogWarning(
                    "Slow request detected | Method: {Method} | Path: {RequestPath} | Status: {StatusCode} | Duration: {ElapsedMs}ms",
                    method,
                    requestPath,
                    statusCode,
                    elapsedMs);
            }
            else
            {
                this.logger.LogInformation(
                    "Request completed | Method: {Method} | Path: {RequestPath} | Status: {StatusCode} | Duration: {ElapsedMs}ms",
                    method,
                    requestPath,
                    statusCode,
                    elapsedMs);
            }
        }
    }
}