
using Abcmoney_Transfer.Data;
using Abcmoney_Transfer.Models;
using Abcmoney_Transfer.Services;
using AbcmoneyTransfer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using static Abcmoney_Transfer.Models.IIdentity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();

var configuration = builder.Configuration;
//var serviceRegistrar = new ServiceRegistrar();
//serviceRegistrar.Register(builder.Services, configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true, // Ensure the token has not expired
            ValidIssuer = "ABSExchange", // Match the issuer in TokenService
            ValidAudience = "ABSExchange", // Match the audience in TokenService
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // Use the same secret key
        };

        // Optional: Customize token handling events
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Query.ContainsKey("token"))
                {
                    context.Token = context.Request.Query["token"];
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.Response.Headers.Append("Authentication-Failed", "true");
                return Task.CompletedTask;
            }
        };
    });

// Configure Swagger
var app = builder.Build();

// Ensure that DataSeeder runs during app startup
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dataSeeder = serviceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedSuperAdminAsync(serviceProvider);
}
if (app.Environment.IsDevelopment())
{
   
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
