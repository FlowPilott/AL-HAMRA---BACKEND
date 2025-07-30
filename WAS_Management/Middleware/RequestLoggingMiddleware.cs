using Serilog;
using System.Diagnostics;
using System.Text;

namespace WAS_Management.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        // Paths to exclude from detailed logging
        private static readonly HashSet<string> ExcludedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/favicon.ico",
            "/robots.txt",
            "/sitemap.xml",
            "/apple-touch-icon.png",
            "/browserconfig.xml",
            "/manifest.json"
        };

        // Extensions to exclude from detailed logging
        private static readonly HashSet<string> ExcludedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".ico",
            ".png",
            ".jpg",
            ".jpeg",
            ".gif",
            ".svg",
            ".css",
            ".js",
            ".woff",
            ".woff2",
            ".ttf",
            ".eot"
        };

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            // Add request ID to the response headers
            context.Response.Headers.Add("X-Request-ID", requestId);

            // Check if this is a path we should skip detailed logging for
            bool shouldSkipDetailedLogging = ShouldSkipDetailedLogging(context.Request.Path);

            if (!shouldSkipDetailedLogging)
            {
                // Log request details for API calls
                await LogRequest(context, requestId);
            }
            else
            {
                // Simple logging for static files
                _logger.LogDebug("Static file request {RequestId}: {Method} {Path}",
                    requestId, context.Request.Method, context.Request.Path);
            }

            // Capture the original response body stream
            var originalBodyStream = context.Response.Body;

            try
            {
                if (!shouldSkipDetailedLogging)
                {
                    using var responseBody = new MemoryStream();
                    context.Response.Body = responseBody;

                    // Call the next middleware
                    await _next(context);

                    stopwatch.Stop();

                    // Log response details
                    await LogResponse(context, requestId, stopwatch.ElapsedMilliseconds, responseBody);
                    responseBody.Seek(0, SeekOrigin.Begin);
                    // Copy the response back to the original stream
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                else
                {
                    // For static files, just pass through without body capturing
                    await _next(context);
                    stopwatch.Stop();

                    // Simple completion log for static files
                    if (context.Response.StatusCode >= 400)
                    {
                        _logger.LogWarning("Static file request {RequestId} completed with status {StatusCode} in {ElapsedMilliseconds}ms: {Method} {Path}",
                            requestId, context.Response.StatusCode, stopwatch.ElapsedMilliseconds,
                            context.Request.Method, context.Request.Path);
                    }
                    else
                    {
                        _logger.LogDebug("Static file request {RequestId} completed with status {StatusCode} in {ElapsedMilliseconds}ms: {Method} {Path}",
                            requestId, context.Response.StatusCode, stopwatch.ElapsedMilliseconds,
                            context.Request.Method, context.Request.Path);
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                var logLevel = shouldSkipDetailedLogging ? LogLevel.Debug : LogLevel.Error;

                _logger.Log(logLevel, ex,
                    "Request {RequestId} failed after {ElapsedMilliseconds}ms - {Method} {Path}",
                    requestId,
                    stopwatch.ElapsedMilliseconds,
                    context.Request.Method,
                    context.Request.Path);

                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private static bool ShouldSkipDetailedLogging(PathString path)
        {
            var pathValue = path.Value;

            if (string.IsNullOrEmpty(pathValue))
                return false;

            // Check excluded paths
            if (ExcludedPaths.Contains(pathValue))
                return true;

            // Check excluded extensions
            var extension = Path.GetExtension(pathValue);
            if (!string.IsNullOrEmpty(extension) && ExcludedExtensions.Contains(extension))
                return true;

            // Skip static files served from common static directories
            if (pathValue.StartsWith("/css/", StringComparison.OrdinalIgnoreCase) ||
                pathValue.StartsWith("/js/", StringComparison.OrdinalIgnoreCase) ||
                pathValue.StartsWith("/images/", StringComparison.OrdinalIgnoreCase) ||
                pathValue.StartsWith("/img/", StringComparison.OrdinalIgnoreCase) ||
                pathValue.StartsWith("/fonts/", StringComparison.OrdinalIgnoreCase) ||
                pathValue.StartsWith("/assets/", StringComparison.OrdinalIgnoreCase) ||
                pathValue.StartsWith("/static/", StringComparison.OrdinalIgnoreCase) ||
                pathValue.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private async Task LogRequest(HttpContext context, string requestId)
        {
            try
            {
                var request = context.Request;

                // Read request body (for POST/PUT requests)
                string requestBody = string.Empty;
                if (request.ContentLength > 0 &&
                    (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH"))
                {
                    request.EnableBuffering();

                    using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    request.Body.Position = 0;

                    // Truncate large request bodies for logging
                    if (requestBody.Length > 1000)
                    {
                        requestBody = requestBody.Substring(0, 1000) + "... (truncated)";
                    }

                    // Redact sensitive information
                    requestBody = RedactSensitiveData(requestBody);
                }

                _logger.LogInformation(
                    "Request {RequestId} started: {Method} {Path} {QueryString} | " +
                    "ContentType: {ContentType} | ContentLength: {ContentLength} | " +
                    "UserAgent: {UserAgent} | RemoteIP: {RemoteIP} | Body: {RequestBody}",
                    requestId,
                    request.Method,
                    request.Path,
                    request.QueryString,
                    request.ContentType ?? "N/A",
                    request.ContentLength ?? 0,
                    request.Headers["User-Agent"].FirstOrDefault() ?? "N/A",
                    context.Connection.RemoteIpAddress?.ToString() ?? "N/A",
                    string.IsNullOrEmpty(requestBody) ? "N/A" : requestBody
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log request details for {RequestId}", requestId);
            }
        }

        private async Task LogResponse(HttpContext context, string requestId, long elapsedMilliseconds, MemoryStream responseBody)
        {
            try
            {
                var response = context.Response;

                // Read response body
                string responseContent = string.Empty;
                if (responseBody.Length > 0)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    responseContent = await new StreamReader(responseBody).ReadToEndAsync();

                    // Truncate large response bodies for logging
                    if (responseContent.Length > 1000)
                    {
                        responseContent = responseContent.Substring(0, 1000) + "... (truncated)";
                    }

                    // Redact sensitive information from response
                    responseContent = RedactSensitiveData(responseContent);
                }

                var logLevel = GetLogLevel(response.StatusCode, elapsedMilliseconds);

                _logger.Log(logLevel,
                    "Request {RequestId} completed: {StatusCode} | " +
                    "ElapsedMs: {ElapsedMilliseconds} | ContentType: {ContentType} | " +
                    "ContentLength: {ContentLength} | Body: {ResponseBody}",
                    requestId,
                    response.StatusCode,
                    elapsedMilliseconds,
                    response.ContentType ?? "N/A",
                    responseBody.Length,
                    string.IsNullOrEmpty(responseContent) ? "N/A" : responseContent
                );

                // Log performance warnings
                if (elapsedMilliseconds > 5000)
                {
                    _logger.LogWarning(
                        "Slow request detected - {RequestId} took {ElapsedMilliseconds}ms to complete",
                        requestId, elapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log response details for {RequestId}", requestId);
            }
        }

        private static string RedactSensitiveData(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            // Redact common sensitive fields (case-insensitive)
            var sensitivePatterns = new[]
            {
                @"""password""\s*:\s*""[^""]*""",
                @"""Password""\s*:\s*""[^""]*""",
                @"""token""\s*:\s*""[^""]*""",
                @"""Token""\s*:\s*""[^""]*""",
                @"""secret""\s*:\s*""[^""]*""",
                @"""Secret""\s*:\s*""[^""]*""",
                @"""authorization""\s*:\s*""[^""]*""",
                @"""Authorization""\s*:\s*""[^""]*""",
                @"""apikey""\s*:\s*""[^""]*""",
                @"""ApiKey""\s*:\s*""[^""]*""",
                @"""creditcard""\s*:\s*""[^""]*""",
                @"""CreditCard""\s*:\s*""[^""]*"""
            };

            foreach (var pattern in sensitivePatterns)
            {
                content = System.Text.RegularExpressions.Regex.Replace(
                    content,
                    pattern,
                    match => match.Value.Substring(0, match.Value.IndexOf(':') + 1) + " \"[REDACTED]\"",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }

            return content;
        }

        private static LogLevel GetLogLevel(int statusCode, long elapsedMilliseconds)
        {
            if (statusCode >= 500)
                return LogLevel.Error;

            if (statusCode >= 400)
                return LogLevel.Warning;

            if (elapsedMilliseconds > 10000)
                return LogLevel.Warning;

            return LogLevel.Information;
        }
    }

    // Extension method to easily add the middleware
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}