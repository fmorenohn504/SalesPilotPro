using Microsoft.Extensions.Caching.Memory;

namespace SalesPilotPro.Api.Security;

public sealed class DevSessionValidationClient : ISessionValidationClient
{
    private readonly IMemoryCache _cache;

    public DevSessionValidationClient(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<bool> ValidateAsync(
        Guid tenantId,
        Guid sessionId,
        Guid userId,
        CancellationToken ct)
    {
        var cacheKey = $"sid:{sessionId}";

        if (_cache.TryGetValue(cacheKey, out bool valid))
            return Task.FromResult(valid);

        valid = true;

        _cache.Set(
            cacheKey,
            valid,
            TimeSpan.FromSeconds(45));

        return Task.FromResult(valid);
    }
}
