using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using HairBooking__API.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var secretKey = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("SecretKey is missing in appsettings.json!");
var issuer = configuration["Jwt:Issuer"];
var audience = configuration["Jwt:Audience"];

// cấu hình JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            // 🛠 Đảm bảo lấy role đúng format
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"

        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "admin"));
});

builder.Services.AddControllers();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
