using System.Diagnostics;

namespace SalesPilotPro.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId =
            context.Request.Headers.TryGetValue(HeaderName, out var value) &&
            !string.IsNullOrWhiteSpace(value)
                ? value.ToString()
                : Guid.NewGuid().ToString();

        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (Activity.Current = new Activity("Request"))
        {
            Activity.Current.SetIdFormat(ActivityIdFormat.W3C);
            Activity.Current.Start();
            Activity.Current.AddTag(HeaderName, correlationId);

            await _next(context);
        }
    }
}
