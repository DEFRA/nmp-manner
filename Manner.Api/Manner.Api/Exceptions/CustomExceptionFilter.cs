using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Manner.Application.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Manner.Application.Exceptions;

namespace Manner.Api.Exceptions;

public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var code = HttpStatusCode.InternalServerError;

        if (exception is ArgumentException) code = HttpStatusCode.BadRequest;
        if (exception is KeyNotFoundException) code = HttpStatusCode.NotFound;        
        var result = new JsonResult(new StandardResponse
        {
            Success = false,
            Message = exception.Message,
            Data = null,
            Errors =
                {
                   exception.InnerException?.Message?? exception.Message
                }
        });

        result.StatusCode = (int)code;
        context.Result = result;
        context.ExceptionHandled = true;
    }
}
