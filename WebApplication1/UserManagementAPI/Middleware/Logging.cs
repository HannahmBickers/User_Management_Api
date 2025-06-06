using System.Diagnostics;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Log request details
            Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");

            // Copy response to log it
            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next(context);  // Call the next middleware

            newBody.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(newBody).ReadToEnd();
            newBody.Seek(0, SeekOrigin.Begin);

            // Log response
            Console.WriteLine($"Outgoing Response: {context.Response.StatusCode}");

            await newBody.CopyToAsync(originalBody);  // Write back to response stream
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
