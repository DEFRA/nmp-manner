using Manner.Application.DTOs;
using Newtonsoft.Json;
using System.Net;

namespace Manner.Api.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (exception is ArgumentException) ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            StandardResponse standardResponse = new StandardResponse();
            standardResponse.Success = false;
            standardResponse.Message = exception.Message ?? "Internal Server Error";
            standardResponse.Errors.Add(exception.InnerException?.Message ?? string.Empty);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(standardResponse);
            return context.Response.WriteAsync(json);
        }
                
    }
}
