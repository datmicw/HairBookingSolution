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

            // 🛠 Đảm bảo lấy role đúng format
            RoleClaimType = ClaimTypes.Role
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
});

// ✅ Cấu hình Swagger (với JWT)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HairBooking API",
        Version = "v1",
        Description = "API for HairBooking App",
    });

    // 🛠 Cấu hình Swagger hỗ trợ Authorization (Bearer Token)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your_token}' to authenticate."
    });

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
            new string[] {}
        }
    });
});

// ✅ Thêm các dịch vụ API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Thêm các dịch vụ Custom
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<StoreService>();

var app = builder.Build();

// ✅ Cấu hình Swagger UI
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HairBooking API v1");
        c.RoutePrefix = string.Empty; // Truy cập Swagger tại http://localhost:5000/
    });
}
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins); // Áp dụng CORS
app.UseAuthorization();

// ✅ Kích hoạt Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map API Controllers
app.MapControllers();

app.Run();
