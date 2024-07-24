using Manner.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{builder.Configuration["CustomerIdentityInstance"]}{builder.Configuration["CustomerIdentityDomain"]}/{builder.Configuration["CustomerIdentityPolicyId"]}/v2.0/";
                options.Audience = builder.Configuration["CustomerIdentityClientId"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };

                options.Events = new CustomJwtBearerEvents();
            });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//app.UseSwaggerUI(c =>
//{
//    // Change the URL path here        
//    c.SwaggerEndpoint("/new-swagger-url/v1/swagger.json", "My API V1");
//    // Change the route prefix if you want to access Swagger UI at a different URL 
//    c.RoutePrefix = "new-swagger-url";
//});
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
