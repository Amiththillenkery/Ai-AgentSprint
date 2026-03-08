using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CreateADotnetRepository.Tests
{
    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _next = (HttpContext context) => Task.CompletedTask;
        }

        [Fact]
        public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
        {
            // Arrange
            var middleware = new GlobalExceptionMiddleware(_next, _loggerMock.Object);
            var context = new DefaultHttpContext();
            _next = (HttpContext ctx) => throw new ValidationException("Validation failed");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
            _loggerMock.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ReturnsNotFound()
        {
            // Arrange
            var middleware = new GlobalExceptionMiddleware(_next, _loggerMock.Object);
            var context = new DefaultHttpContext();
            _next = (HttpContext ctx) => throw new NotFoundException("Resource not found");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
            _loggerMock.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_GeneralException_ReturnsInternalServerError()
        {
            // Arrange
            var middleware = new GlobalExceptionMiddleware(_next, _loggerMock.Object);
            var context = new DefaultHttpContext();
            _next = (HttpContext ctx) => throw new Exception("General error");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            _loggerMock.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }

    // Custom exception classes for testing
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    // GlobalExceptionMiddleware implementation
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
            catch (ValidationException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Validation error occurred.");
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("Resource not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}