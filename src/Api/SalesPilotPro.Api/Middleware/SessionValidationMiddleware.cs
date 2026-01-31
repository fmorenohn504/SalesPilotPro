using SalesPilotPro.Api.Security;
using System.Security.Claims;

namespace SalesPilotPro.Api.Middleware;

public sealed class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ISessionValidationClient sessionClient,
        CancellationToken ct)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var tid = context.User.FindFirstValue("tid");
        var sid = context.User.FindFirstValue("sid");
        var sub = context.User.FindFirstValue("sub");

        if (!Guid.TryParse(tid, out var tenantId) ||
            !Guid.TryParse(sid, out var sessionId) ||
            !Guid.TryParse(sub, out var userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var valid = await sessionClient.ValidateAsync(
            tenantId,
            sessionId,
            userId,
            ct);

        if (!valid)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
