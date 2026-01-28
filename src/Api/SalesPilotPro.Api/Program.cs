using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SalesPilotPro.Api.Middleware;
using SalesPilotPro.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger (solo desarrollo)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure
builder.Services.AddInfrastructure();

// 🔐 JWT Authentication (DEV)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("DEV_ONLY_SECRET_KEY_CHANGE_LATER"))
        };
    });

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();


app.UseHttpsRedirection();

// 🔐 Middleware JWT (construye Tenant/User/Module Contexts)
app.UseMiddleware<JwtContextMiddleware>();

// ⚠️ OJO: aún NO activamos UseAuthentication()
// (eso es el PASO 1.8 – punto 2)

// Auth vendrá después
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();
