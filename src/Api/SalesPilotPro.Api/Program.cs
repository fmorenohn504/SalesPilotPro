using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using SalesPilotPro.Api.Contexts;
using SalesPilotPro.Api.Middleware;
using SalesPilotPro.Api.Security; // 🔧 ESTE USING FALTABA
using SalesPilotPro.Core.Contexts;
using SalesPilotPro.Core.Security;
using SalesPilotPro.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

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

// =======================
// SERVICES
// =======================

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ITenantContext, DevTenantContext>();
builder.Services.AddScoped<ISessionValidationClient, DevSessionValidationClient>();

// 🔐 JWT Provider
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SalesPilotPro CRM API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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

builder.Services.AddInfrastructure(builder.Configuration);

// =======================
// AUTHENTICATION (JWT)
// =======================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
                ),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "sub"
        };
    });

// =======================
// AUTHORIZATION
// =======================

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("MODULE_CRM",
        policy => policy.RequireClaim("mods", "CRM"));
});

// =======================
// API VERSIONING
// =======================

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

// =======================
// HEALTH
// =======================

builder.Services.AddHealthChecks();

// =======================
// RATE LIMITING
// =======================

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = ratePermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateWindowSeconds);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

// =======================
// CORS
// =======================

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =======================
// LOGGING
// =======================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/salespilotpro-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// =======================
// PIPELINE
// =======================

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("default");
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<TenantGateMiddleware>();
app.UseMiddleware<SessionValidationMiddleware>();
app.UseAuthorization();

app.UseRateLimiter();

app.MapHealthChecks("/health").AllowAnonymous();
app.MapControllers();

app.Run();
