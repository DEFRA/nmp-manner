using Manner.Api.Exceptions;
using Manner.Api.Helpers;
using Manner.Api.Security;
using Manner.Api.Validations;
using Manner.Infrastructure.Configuration;
using Manner.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = $"{builder.Configuration["CustomerIdentityInstance"]}{builder.Configuration["CustomerIdentityDomain"]}/{builder.Configuration["CustomerIdentityPolicyId"]}/v2.0/";
                    options.Audience = builder.Configuration["CustomerIdentityClientId"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        ValidateIssuer = false
                    };

                    options.Events = new CustomJwtBearerEvents();
                });

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Manner API", Version = "v1" });

    // Define the Bearer Token authentication scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Apply the Bearer Token authentication scheme to all operations
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
       {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new List<string>()
       }
     });
    c.EnableAnnotations();
});

builder.Services.RegisterServices(builder.Configuration);
builder.Services.RegisterRepositories(builder.Configuration);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Manner API V1");
});


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//}
app.UseMiddleware<ValidationMiddleware>();

// Register the exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
