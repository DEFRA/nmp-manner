using FluentValidation.Results;
using Manner.Application.DTOs;
using Newtonsoft.Json;

namespace Manner.Api.Validations;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        if (!context.Request.HasJsonContentType())
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
        {            
            try
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
                var validationFailures = JsonConvert.DeserializeObject<ValidationError>(body);
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                if (validationFailures != null)
                {
                    context.Response.StatusCode = validationFailures.Status;
                }
                var standardResponse = new StandardResponse
                {
                    Success = false,
                    Message = "Validation failed"
                };
                if (validationFailures != null)
                {
                    foreach (var error in validationFailures.Errors)
                    {
                        foreach (var value in error.Value)
                        {
                            standardResponse.Errors.Add($"{error.Key} - {value}");
                        }
                    }
                }
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(standardResponse));
            }
            catch (Exception)
            {
               
            }
            
        }
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
}
