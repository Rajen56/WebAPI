using Abcmoney_Transfer.Data;
using Abcmoney_Transfer.Models;
using Abcmoney_Transfer.Services;
using ABCmoneysend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Abcmoney_Transfer.Models.IdentityModel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(); // This registers ITempDataDictionaryFactory
// OR
builder.Services.AddMvc(); // This also registers the required services

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<DataSeeder>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity with custom AppUser & AppRole
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpContextAccessor();

// Configure JWT Authentication
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
            ValidateLifetime = true,
            ValidIssuer = "ABSExchange",
            ValidAudience = "ABSExchange",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

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

var app = builder.Build();

// Run Seeder
//using (var scope = app.Services.CreateScope())
//{
//    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
//    await dataSeeder.SeedSuperAdminAsync(scope.ServiceProvider);
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
              name: "default",
              pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
