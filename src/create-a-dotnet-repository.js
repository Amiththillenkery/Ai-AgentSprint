csharp
// File: Services/RepositoryService.cs

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class RepositoryServiceOptions
    {
        public string ConnectionString { get; set; }
    }

    public interface IRepositoryService<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }

    public class RepositoryService<T> : IRepositoryService<T>
    {
        private readonly RepositoryServiceOptions _options;

        public RepositoryService(IOptions<RepositoryServiceOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Simulate data retrieval
            await Task.Delay(100); // Simulate async operation
            return new List<T>(); // Return an empty list for demonstration
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            // Simulate data retrieval
            await Task.Delay(100); // Simulate async operation
            return default(T); // Return default value for demonstration
        }

        public async Task AddAsync(T entity)
        {
            // Simulate data addition
            await Task.Delay(100); // Simulate async operation
        }

        public async Task UpdateAsync(T entity)
        {
            // Simulate data update
            await Task.Delay(100); // Simulate async operation
        }

        public async Task DeleteAsync(Guid id)
        {
            // Simulate data deletion
            await Task.Delay(100); // Simulate async operation
        }
    }
}

// File: Middleware/ExceptionHandlingMiddleware.cs

using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Middleware
{
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = new { message = exception.Message };
            return context.Response.WriteAsJsonAsync(result);
        }
    }
}

// File: Program.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Services;
using Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.Configure<RepositoryServiceOptions>(
    builder.Configuration.GetSection("RepositoryService"));

builder.Services.AddScoped(typeof(IRepositoryService<>), typeof(RepositoryService<>));

var app = builder.Build();

// Configure middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure endpoints
app.MapGet("/", () => "Hello World!");

app.Run();

on
// File: appsettings.json

{
  "RepositoryService": {
    "ConnectionString": "YourConnectionStringHere"
  }
}