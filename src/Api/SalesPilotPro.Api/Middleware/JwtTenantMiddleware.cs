using System.Security.Claims;
using SalesPilotPro.Infrastructure.Persistence;

namespace SalesPilotPro.Api.Middleware;

public sealed class JwtTenantMiddleware
{
    private readonly RequestDelegate _next;

    public JwtTenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, SalesPilotProDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenantId")?.Value;

            if (Guid.TryParse(tenantClaim, out var tenantId))
            {
                db.TenantId = tenantId;
            }
        }

        await _next(context);
    }
}
