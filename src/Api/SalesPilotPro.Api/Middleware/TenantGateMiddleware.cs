using System.Security.Claims;

namespace SalesPilotPro.Api.Middleware;

public sealed class TenantGateMiddleware
{
    private readonly RequestDelegate _next;

    public TenantGateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tidClaim = context.User.FindFirst("tid")?.Value;

        if (string.IsNullOrWhiteSpace(tidClaim) || !Guid.TryParse(tidClaim, out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items["TenantId"] = tenantId;

        await _next(context);
    }
}
