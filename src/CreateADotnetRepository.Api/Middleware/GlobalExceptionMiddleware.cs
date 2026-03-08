using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CreateADotnetRepository.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var correlationId = context.TraceIdentifier;
            var statusCode = HttpStatusCode.InternalServerError;
            var problemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.com/500",
                Title = "An unexpected error occurred.",
                Status = (int)statusCode,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            switch (exception)
            {
                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    problemDetails.Title = "Validation error.";
                    problemDetails.Type = "https://httpstatuses.com/400";
                    break;
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    problemDetails.Title = "Resource not found.";
                    problemDetails.Type = "https://httpstatuses.com/404";
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Unauthorized access.";
                    problemDetails.Type = "https://httpstatuses.com/401";
                    break;
                default:
                    _logger.LogError(exception, "An unexpected error occurred. Correlation ID: {CorrelationId}", correlationId);
                    break;
            }

            problemDetails.Status = (int)statusCode;
            problemDetails.Extensions["correlationId"] = correlationId;

            context.Response.StatusCode = (int)statusCode;
            var jsonResponse = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class ValidationException : Exception { }
    public class NotFoundException : Exception { }
}