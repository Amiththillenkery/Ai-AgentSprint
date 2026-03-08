using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using ImplementArticleEntitiy.Api.Middlewares;
using FluentValidation;

namespace ImplementArticleEntitiy.Tests
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
        public async Task InvokeAsync_ShouldReturn400_WhenValidationExceptionIsThrown()
        {
            var middleware = new GlobalExceptionMiddleware(_next, _loggerMock.Object);
            var context = new DefaultHttpContext();

            _next = (HttpContext context) => throw new ValidationException("Validation failed");

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            context.Response.ContentType.Should().Be("application/problem+json");
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn404_WhenNotFoundExceptionIsThrown()
        {
            var middleware = new GlobalExceptionMiddleware(_next, _loggerMock.Object);
            var context = new DefaultHttpContext();

            _next = (HttpContext context) => throw new NotFoundException("Resource not found");

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            context.Response.ContentType.Should().Be("application/problem+json");
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn500_WhenGeneralExceptionIsThrown()
        {
            var middleware = new GlobalExceptionMiddleware(_next, _loggerMock.Object);
            var context = new DefaultHttpContext();

            _next = (HttpContext context) => throw new Exception("An error occurred");

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            context.Response.ContentType.Should().Be("application/problem+json");
        }
    }
}