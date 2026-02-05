using SalesPilotPro.Core.Security;

namespace SalesPilotPro.Api.Security;

public sealed class DevSessionValidationClient : ISessionValidationClient
{
    public Task<bool> IsSessionValidAsync(
        Guid tenantId,
        Guid sessionId,
        Guid actorUserId,
        CancellationToken ct)
    {
        return Task.FromResult(true);
    }
}
