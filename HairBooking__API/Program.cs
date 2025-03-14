using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using HairBooking__API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// 🛠 Lấy cấu hình JWT từ `appsettings.json`
var secretKey = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("SecretKey is missing in appsettings.json!");
var issuer = configuration["Jwt:Issuer"];
var audience = configuration["Jwt:Audience"];

// ✅ Cấu hình JWT Authentication
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
            RoleClaimType = ClaimTypes.Role // Sử dụng Claim Role để xác định quyền
        };
    });
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Cho phép React truy cập
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ✅ Cấu hình Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "admin"));
    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "user"));
    options.AddPolicy("OwnerStore", policy =>
        policy.RequireClaim(ClaimTypes.Role, "owner"));
});

// ✅ Thêm các dịch vụ API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Thêm các dịch vụ Custom
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<StoreService>();

var app = builder.Build();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins); // Áp dụng CORS
app.UseAuthorization();

// ✅ Kích hoạt Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map API Controllers
app.MapControllers();

app.Run();
