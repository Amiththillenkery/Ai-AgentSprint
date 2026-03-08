using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StructureTheCurrentRepositoryWithAddingSolut.Api.Middleware;

namespace StructureTheCurrentRepositoryWithAddingSolut.Api.Middleware
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
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "An unexpected error occurred!",
                Detail = exception.Message,
                Extensions = { ["correlationId"] = correlationId }
            };

            switch (exception)
            {
                case ValidationException validationException:
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Validation error";
                    problemDetails.Detail = validationException.Message;
                    break;

                case NotFoundException:
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Resource not found";
                    break;

                case UnauthorizedAccessException:
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Unauthorized access";
                    break;

                default:
                    _logger.LogError(exception, "Unhandled exception occurred.");
                    break;
            }

            context.Response.StatusCode = problemDetails.Status.Value;
            var jsonResponse = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}