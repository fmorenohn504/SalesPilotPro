using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using SalesPilotPro.Core.Security;
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
        IMemoryCache cache)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        if (context.User?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var tenantId = context.User.FindFirstValue("tid");
        var sessionId = context.User.FindFirstValue("sid");
        var userId = context.User.FindFirstValue("sub");

        if (tenantId is null || sessionId is null || userId is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var cacheKey = $"session:{sessionId}";

        if (!cache.TryGetValue(cacheKey, out bool valid))
        {
            valid = await sessionClient.IsSessionValidAsync(
                Guid.Parse(tenantId),
                Guid.Parse(sessionId),
                Guid.Parse(userId),
                context.RequestAborted);

            cache.Set(cacheKey, valid, TimeSpan.FromSeconds(45));
        }

        if (!valid)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
