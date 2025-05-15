using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// เพิ่มระบบ Controller เข้าไปใน Service Collection
// ทำให้ใช้ [ApiController], ControllerBase, MapControllers() ได้

builder.Services.AddEndpointsApiExplorer();
// ใช้เพื่อให้ Swagger หา endpoint ทั้งหมดจาก controller หรือ minimal API ได้

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Main API", Version = "v1" });

    // เพิ่มส่วนนี้เพื่อให้ Swagger รองรับ Authorization ด้วย Bearer Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token (with Bearer prefix)",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});
// เปิดการใช้งาน Swagger UI (หน้าทดสอบ API อัตโนมัติ) พร้อมรองรับ Authorization

var key = builder.Configuration["JwtSettings:SecretKey"];

if (string.IsNullOrEmpty(key))
{
    throw new Exception("JWT SecretKey is not configured!");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });
// ทำการ Authorization

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
