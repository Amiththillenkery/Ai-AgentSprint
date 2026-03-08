using System.Net;
using System.Threading.Tasks;
using ImplementArticleEntitiy.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using System.Text.Json;
using FluentValidation;
using System.Collections.Generic;

namespace ImplementArticleEntitiy.Tests
{
    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _next;
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _logger;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareTests()
        {
            _next = new Mock<RequestDelegate>();
            _logger = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _middleware = new GlobalExceptionMiddleware(_next.Object, _logger.Object);
        }

        [Fact]
        public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
        {
            _next.Setup(n => n(It.IsAny<HttpContext>())).Throws(new ValidationException("Validation error"));

            var context = new DefaultHttpContext();
            await _middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);
            response.Should().NotBeNull();
            response!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ReturnsNotFound()
        {
            _next.Setup(n => n(It.IsAny<HttpContext>())).Throws(new KeyNotFoundException("Not found"));

            var context = new DefaultHttpContext();
            await _middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);
            response.Should().NotBeNull();
            response!.Status.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task InvokeAsync_GeneralException_ReturnsInternalServerError()
        {
            _next.Setup(n => n(It.IsAny<HttpContext>())).Throws(new System.Exception("General error"));

            var context = new DefaultHttpContext();
            await _middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);
            response.Should().NotBeNull();
            response!.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}