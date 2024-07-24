using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Manner.Api.Security
{

    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            // Override the response status code and message
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            context.Response.WriteAsync("{\"error\":\"You are not authorized to access this resource.\"}");

            return Task.CompletedTask;
        }        
    }
}
