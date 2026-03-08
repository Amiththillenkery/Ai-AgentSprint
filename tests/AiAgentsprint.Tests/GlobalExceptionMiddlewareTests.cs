using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using StructureTheCurrentRepositoryWithAddingSolut.Tests;
using FluentValidation;

namespace StructureTheCurrentRepositoryWithAddingSolut.Tests
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
            var context = new DefaultHttpContext();
            _nextMock.Setup(n => n.Invoke(It.IsAny<HttpContext>())).ThrowsAsync(new ValidationException("Validation error"));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            _loggerMock.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<ValidationException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ReturnsNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _nextMock.Setup(n => n.Invoke(It.IsAny<HttpContext>())).ThrowsAsync(new NotFoundException("Not found"));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            _loggerMock.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<NotFoundException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_GeneralException_ReturnsInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _nextMock.Setup(n => n.Invoke(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("General error"));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            _loggerMock.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}