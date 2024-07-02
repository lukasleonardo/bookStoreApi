using System.Net;
using bookStoreApi.Handler;
using Newtonsoft.Json;

namespace bookStoreApi.Handlers
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
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

            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is NotFoundException)
            {
                code = HttpStatusCode.NotFound; // 404
            }
            else if (exception is ServiceException)
            {
                code = HttpStatusCode.BadRequest; // 400
            }

            context.Response.StatusCode = (int)code;

            var result = JsonConvert.SerializeObject(new { erro = exception.Message });
            return context.Response.WriteAsync(result);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }

}
