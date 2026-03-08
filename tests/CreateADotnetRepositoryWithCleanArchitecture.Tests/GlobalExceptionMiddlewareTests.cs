using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CreateADotnetRepositoryWithCleanArchitecture.Tests
{
    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _middleware = new GlobalExceptionMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(new ValidationException("Validation error"));
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
            _loggerMock.Verify(log => log.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ReturnsNotFound()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(new NotFoundException("Not found"));
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
            _loggerMock.Verify(log => log.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_GeneralException_ReturnsInternalServerError()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(new Exception("General error"));
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            _loggerMock.Verify(log => log.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
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
}