using SalesPilotPro.Api.Contexts;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Middleware;

public sealed class JwtContextMiddleware
{
    private readonly RequestDelegate _next;

    public JwtContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst("tid")?.Value;
            var tenantCodeClaim = context.User.FindFirst("tcode")?.Value ?? "DEV";

            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                context.Items[nameof(ITenantContext)] =
                    new TenantContext(tenantId, tenantCodeClaim);
            }
        }

        await _next(context);
    }
}
