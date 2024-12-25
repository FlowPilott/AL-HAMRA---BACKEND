using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WAS_Management.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            //_logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //var traceId = Guid.NewGuid();
                //var innerExceptionMessage = ex.InnerException != null ? $" Inner Exception: {ex.InnerException.Message}" : string.Empty;

                //// Log detailed error information
                //////_logger.LogError($"Error occurred while processing the request. TraceId: {traceId}, " +
                ////                 $"Message: {ex.Message}, Inner Message: {innerExceptionMessage}, " +
                ////                 $"StackTrace: {ex.StackTrace}");

                //// Determine the response status code based on exception type
                //var statusCode = ex switch
                //{
                //    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                //    ArgumentException => StatusCodes.Status400BadRequest,
                //    KeyNotFoundException => StatusCodes.Status404NotFound,
                //    _ => StatusCodes.Status500InternalServerError // Default to 500 for unhandled exceptions
                //};
                //// Clear any existing response headers if they were already set by another middleware
                //context.Response.Clear();
                //context.Response.StatusCode = statusCode;
                //context.Response.ContentType = "application/json"; // Set content type to JSON
                //var typeUri = statusCode switch
                //{
                //    StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                //    StatusCodes.Status401Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
                //    StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                //    _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1" // Default for 500 errors
                //};

                //// Prepare ProblemDetails with detailed information
                //var problemDetails = new ProblemDetails
                //{
                //    Type = typeUri,
                //    Title = statusCode == StatusCodes.Status500InternalServerError
                //        ? "Internal Server Error"
                //        : ex.GetType().Name,
                //    Status = statusCode,
                //    Instance = context.Request.Path,
                //    Detail = $"Message: {ex.Message}. TraceId: {traceId}. Inner Exception: {innerExceptionMessage}"
                //};

                //var jsonResponse = JsonSerializer.Serialize(problemDetails); // Serialize to JSON
                await context.Response.WriteAsync(ex.Message); // Write the response
            }
        }
    }
}
