using Microsoft.AspNetCore.Authorization;
using SalesPilotPro.Api.Contexts;
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

        // ⬇️ RESPETAR AllowAnonymous
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

        var tenantIdClaim = context.User.FindFirst("tid")?.Value;
        var tenantCodeClaim = context.User.FindFirst("tcode")?.Value ?? "DEV";

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items[nameof(ITenantContext)] =
            new TenantContext(tenantId, tenantCodeClaim);

        await _next(context);
    }
}
