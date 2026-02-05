using Microsoft.AspNetCore.Authorization;
using SalesPilotPro.Core.Contexts;

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
        var endpoint = context.GetEndpoint();

        // Permitir endpoints explícitamente anónimos (login-dev, health, etc.)
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var tenantClaim = context.User.FindFirst("tid")?.Value;

        if (!Guid.TryParse(tenantClaim, out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items[nameof(ITenantContext)] = new TenantContext(tenantId);

        await _next(context);
    }
}
