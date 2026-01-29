using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SalesPilotPro.Api.Contexts;
using SalesPilotPro.Api.Middleware;
using SalesPilotPro.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//
// 🔧 SERVICES
//

builder.Services.AddScoped<SalesPilotPro.Core.Contexts.ITenantContext>(_ =>
    new DevTenantContext());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("DEV_ONLY_SECRET_KEY_CHANGE_LATER"))
        };
    });

// 🔐 Authorization (Roles + Modules)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MODULE_crm",
        policy => policy.RequireClaim("module", "crm"));

    options.AddPolicy("MODULE_reports",
        policy => policy.RequireClaim("module", "reports"));

    options.AddPolicy("MODULE_admin",
        policy => policy.RequireClaim("module", "admin"));
});

var app = builder.Build();

//
// 🔁 PIPELINE
//
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<JwtContextMiddleware>();

app.UseAuthentication();
app.UseMiddleware<JwtTenantMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
