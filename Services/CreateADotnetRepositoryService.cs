csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Services
{
    public interface ICreateADotnetRepositoryService
    {
        void InitializeRepository();
    }

    public class CreateADotnetRepositoryService : ICreateADotnetRepositoryService
    {
        private readonly IConfiguration _configuration;

        public CreateADotnetRepositoryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void InitializeRepository()
        {
            try
            {
                // Repository initialization logic
                var repositoryPath = _configuration["Repository:Path"];
                if (string.IsNullOrEmpty(repositoryPath))
                {
                    throw new InvalidOperationException("Repository path is not configured.");
                }

                // Logic to create and initialize the repository
                Console.WriteLine($"Repository initialized at {repositoryPath}");
            }
            catch (Exception ex)
            {
                // Log exception and handle it appropriately
                Console.WriteLine($"An error occurred while initializing the repository: {ex.Message}");
                throw;
            }
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICreateADotnetRepositoryService, CreateADotnetRepositoryService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var repoService = context.RequestServices.GetRequiredService<ICreateADotnetRepositoryService>();
                    repoService.InitializeRepository();
                    await context.Response.WriteAsync("Repository Initialized");
                });
            });
        }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log exception and return a generic error response
                Console.WriteLine($"An unhandled exception occurred: {ex.Message}");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}