csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Services
{
    public interface ICreateADotnetRepositoryService
    {
        void InitializeRepository();
    }

    public class CreateADotnetRepositoryService : ICreateADotnetRepositoryService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateADotnetRepositoryService> _logger;

        public CreateADotnetRepositoryService(IConfiguration configuration, ILogger<CreateADotnetRepositoryService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void InitializeRepository()
        {
            try
            {
                var repoPath = _configuration.GetValue<string>("RepositorySettings:Path");
                if (string.IsNullOrEmpty(repoPath))
                {
                    throw new InvalidOperationException("Repository path is not configured.");
                }

                // Logic to initialize the repository
                _logger.LogInformation($"Repository initialized at {repoPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the repository.");
                throw;
            }
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseMiddleware<ExceptionMiddleware>(logger);
        }
    }

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware."
            }.ToString());
        }
    }

    public static class ServiceExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICreateADotnetRepositoryService, CreateADotnetRepositoryService>();
            services.Configure<RepositorySettings>(configuration.GetSection("RepositorySettings"));
        }
    }

    public class RepositorySettings
    {
        public string Path { get; set; }
    }
}