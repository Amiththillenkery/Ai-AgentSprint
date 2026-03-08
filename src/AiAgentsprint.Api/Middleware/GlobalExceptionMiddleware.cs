using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ImplementArticleEntitiy.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "An unexpected error occurred!",
                Status = (int)statusCode,
                Detail = exception.Message,
                Instance = context.Request.Path,
                Extensions = { ["correlationId"] = correlationId }
            };

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    problemDetails.Title = "Validation error";
                    problemDetails.Status = (int)statusCode;
                    problemDetails.Extensions["errors"] = validationException.Errors;
                    break;
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    problemDetails.Title = "Resource not found";
                    problemDetails.Status = (int)statusCode;
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Unauthorized access";
                    problemDetails.Status = (int)statusCode;
                    break;
                default:
                    _logger.LogError(exception, "An unhandled exception occurred.");
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            var result = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(result);
        }
    }
}