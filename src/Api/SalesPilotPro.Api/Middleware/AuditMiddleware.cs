using System.Security.Claims;
using SalesPilotPro.Infrastructure.Persistence;
using SalesPilotPro.Infrastructure.Entities.Crm;

namespace SalesPilotPro.Api.Middleware;

public sealed class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        SalesPilotProDbContext db,
        CancellationToken ct)
    {
        await _next(context);

        if (!context.User.Identity?.IsAuthenticated ?? true)
            return;

        var tid = context.User.FindFirstValue("tid");
        var sid = context.User.FindFirstValue("sid");
        var sub = context.User.FindFirstValue("sub");

        if (!Guid.TryParse(tid, out var tenantId) ||
            !Guid.TryParse(sid, out var sessionId) ||
            !Guid.TryParse(sub, out var userId))
            return;

        var audit = new AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ActorUserId = userId,
            SessionId = sessionId,
            Action = $"{context.Request.Method} {context.Request.Path}",
            TargetType = null,
            TargetId = null,
            OccurredAtUtc = DateTime.UtcNow,
            CorrelationId = context.Response.Headers["X-Correlation-Id"].ToString(),
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers.UserAgent.ToString()
        };

        db.AuditEvents.Add(audit);
        await db.SaveChangesAsync(ct);
    }
}
