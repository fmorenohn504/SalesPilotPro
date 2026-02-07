using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalesPilotPro.Api.Contexts;
using SalesPilotPro.Api.Middleware;
using SalesPilotPro.Api.Persistence;
using SalesPilotPro.Api.Security;
using SalesPilotPro.Api.Services;
using SalesPilotPro.Api.Services.Interfaces;
using SalesPilotPro.Core.Contexts;
using SalesPilotPro.Core.Security;
using SalesPilotPro.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

// =======================
// CONFIG
// =======================

var allowedOrigins =
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

var ratePermitLimit =
    builder.Configuration.GetValue<int>("RateLimit:PermitLimit", 100);

var rateWindowSeconds =
    builder.Configuration.GetValue<int>("RateLimit:WindowSeconds", 60);

var iamBaseUrl = builder.Configuration["Iam:BaseUrl"];

// =======================
// SERVICES
// =======================

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// -----------------------
// CONTEXTS (DESDE HttpContext)
// -----------------------

builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
builder.Services.AddScoped<IUserContext, HttpUserContext>();
builder.Services.AddScoped<IModuleContext, HttpModuleContext>();

// -----------------------
// SESSION VALIDATION
// -----------------------

if (isDevelopment)
{
    builder.Services.AddScoped<ISessionValidationClient, DevSessionValidationClient>();
}
else
{
    builder.Services.AddHttpClient<HttpSessionValidationClient>(client =>
    {
        if (string.IsNullOrWhiteSpace(iamBaseUrl))
            throw new InvalidOperationException("Iam:BaseUrl is not configured");

        client.BaseAddress = new Uri(iamBaseUrl);
    });

    builder.Services.AddScoped<ISessionValidationClient>(sp =>
        sp.GetRequiredService<HttpSessionValidationClient>());
}

// -----------------------
// DATABASE
// -----------------------

builder.Services.AddDbContext<CrmDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// -----------------------
// SERVICES
// -----------------------

builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddControllers();

// -----------------------
// API VERSIONING (PASIVO – SOLO PARA ESTABILIZAR PIPELINE)
// -----------------------

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = false;
});

// -----------------------
// AUTHENTICATION (JWT)
// -----------------------

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !isDevelopment;
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            ),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "sub"
        };
    });

// -----------------------
// AUTHORIZATION
// -----------------------

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});

// -----------------------
// SWAGGER
// -----------------------

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// -----------------------
// INFRASTRUCTURE
// -----------------------

builder.Services.AddInfrastructure(builder.Configuration);

// =======================
// PIPELINE
// =======================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();

// 👉 AQUI SE POBLA HttpContext.Items
app.UseMiddleware<JwtContextMiddleware>();

app.UseMiddleware<TenantGateMiddleware>();
app.UseMiddleware<SessionValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
