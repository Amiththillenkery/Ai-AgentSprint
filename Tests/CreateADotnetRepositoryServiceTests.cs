csharp
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

public class CreateADotnetRepositoryServiceTests
{
    private readonly Mock<ILogger<CreateADotnetRepositoryService>> _loggerMock;
    private readonly IConfiguration _configuration;
    private readonly CreateADotnetRepositoryService _service;

    public CreateADotnetRepositoryServiceTests()
    {
        _loggerMock = new Mock<ILogger<CreateADotnetRepositoryService>>();
        
        var inMemorySettings = new Dictionary<string, string>
        {
            {"RepositorySettings:RepositoryName", "TestRepo"},
            {"RepositorySettings:EnableFeatureX", "true"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new CreateADotnetRepositoryService(_loggerMock.Object, _configuration);
    }

    [Fact]
    public void Should_CreateRepository_With_ValidConfiguration()
    {
        // Act
        var result = _service.CreateRepository();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestRepo");
        result.IsFeatureXEnabled.Should().BeTrue();
    }

    [Fact]
    public void Should_LogInformation_When_RepositoryCreated()
    {
        // Act
        _service.CreateRepository();

        // Assert
        _loggerMock.Verify(
            x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void Should_HandleException_UsingMiddleware()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var middleware = new ExceptionHandlingMiddleware(async (innerHttpContext) =>
        {
            throw new InvalidOperationException("Test exception");
        });

        // Act
        Func<System.Threading.Tasks.Task> act = async () => await middleware.InvokeAsync(httpContext);

        // Assert
        act.Should().NotThrow();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}

public class CreateADotnetRepositoryService
{
    private readonly ILogger<CreateADotnetRepositoryService> _logger;
    private readonly IConfiguration _configuration;

    public CreateADotnetRepositoryService(ILogger<CreateADotnetRepositoryService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Repository CreateRepository()
    {
        var repoName = _configuration["RepositorySettings:RepositoryName"];
        var enableFeatureX = bool.Parse(_configuration["RepositorySettings:EnableFeatureX"]);

        _logger.LogInformation("Creating repository {RepoName}", repoName);

        return new Repository
        {
            Name = repoName,
            IsFeatureXEnabled = enableFeatureX
        };
    }
}

public class Repository
{
    public string Name { get; set; }
    public bool IsFeatureXEnabled { get; set; }
}

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}