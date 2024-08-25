using Manner.Application.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Encodings.Web;

namespace Manner.Api.Security
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            //Override the response status code and message
            
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            StandardResponse standardResponse = new StandardResponse();
            standardResponse.Success = false;
            standardResponse.Message = context.Error ?? "Unauthorised";
            standardResponse.Errors.Add(context.ErrorDescription ?? "You are not authorised to access this resource.");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(standardResponse);
            context.Response.WriteAsync(json);
            return Task.CompletedTask;
        }


        //public override Task AuthenticationFailed(AuthenticationFailedContext context)
        //{
            
        //    return OnAuthenticationFailed(context);
        //}

        //
        // Summary:
        //     Invoked if Authorization fails and results in a Forbidden response
        //public virtual Task Forbidden(ForbiddenContext context)
        //{
        //    return OnForbidden(context);
        //}

        //
        // Summary:
        //     Invoked when a protocol message is first received.
        //public virtual Task MessageReceived(MessageReceivedContext context)
        //{
        //    return OnMessageReceived(context);
        //}

        //
        // Summary:
        //     Invoked after the security token has passed validation and a ClaimsIdentity has
        //     been generated.
        //public override Task TokenValidated(TokenValidatedContext context)
        //{
        //    return OnTokenValidated(context);
        //}
                
    }
}
