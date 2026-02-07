using Microsoft.AspNetCore.Authorization;
using SalesPilotPro.Api.Services.Interfaces;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Middleware;

public sealed class TenantGateMiddleware
{
    private readonly RequestDelegate _next;

    public TenantGateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuditService auditService)
    {
        var endpoint = context.GetEndpoint();

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

        if (!Guid.TryParse(tenantIdClaim, out var tenantIdFromToken))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // 🔑 Resolver el contexto SOLO aquí
        var tenantContext =
            context.RequestServices.GetRequiredService<ITenantContext>();

        if (tenantContext.TenantId != tenantIdFromToken)
        {
            await auditService.RecordAsync(
                action: "Security.CrossTenantAccess",
                targetType: "Tenant",
                targetId: tenantIdFromToken.ToString()
            );

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _next(context);
    }
}
