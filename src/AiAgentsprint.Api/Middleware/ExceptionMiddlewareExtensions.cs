using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace StructureTheCurrentRepositoryWithAddingSolut.Api.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var logger = app.ApplicationServices.GetRequiredService<ILogger<ExceptionMiddlewareExtensions>>();
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        var errorResponse = new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    }
                });
            });
        }
    }
}