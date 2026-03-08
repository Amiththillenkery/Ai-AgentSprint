using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CreateADotnetRepositoryWithCleanArchitecture.Api.Middleware
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
                _logger.LogError(ex, "An exception occurred while processing the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.TraceIdentifier;
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Title = "An error occurred while processing your request.",
                Detail = exception.Message,
                Extensions = { { "correlationId", correlationId } }
            };

            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case ValidationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Validation Error";
                    break;
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Resource Not Found";
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Unauthorized Access";
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(result);
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}