csharp
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public class CreateADotnetRepositoryServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IRepository> _mockRepository;

    public CreateADotnetRepositoryServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockRepository = new Mock<IRepository>();
    }

    [Fact]
    public async Task MinimalApi_ShouldReturnSuccessResponse()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(_mockRepository.Object);
        var app = builder.Build();

        app.MapGet("/repository", async (IRepository repository) =>
        {
            var result = await repository.GetDataAsync();
            return Results.Ok(result);
        });

        // Act
        var response = await app.GetTestClient().GetAsync("/repository");

        // Assert
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public void Configuration_ShouldLoadFromAppSettings()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string> {
            {"RepositorySettings:ConnectionString", "TestConnectionString"}
        };

        _mockConfiguration.Setup(c => c.GetSection("RepositorySettings")["ConnectionString"])
                          .Returns(inMemorySettings["RepositorySettings:ConnectionString"]);

        // Act
        var connectionString = _mockConfiguration.Object.GetSection("RepositorySettings")["ConnectionString"];

        // Assert
        connectionString.Should().Be("TestConnectionString");
    }

    [Fact]
    public async Task GenericExceptionMiddleware_ShouldHandleExceptions()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(_mockRepository.Object);
        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.MapGet("/repository", async (IRepository repository) =>
        {
            throw new System.Exception("Test Exception");
        });

        // Act
        var response = await app.GetTestClient().GetAsync("/repository");

        // Assert
        response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}

public interface IRepository
{
    Task<string> GetDataAsync();
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
        catch (System.Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}