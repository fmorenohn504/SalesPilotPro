using SalesPilotPro.Core.Security;

namespace SalesPilotPro.Api.Security;

public sealed class HttpSessionValidationClient : ISessionValidationClient
{
    public Task<bool> IsSessionValidAsync(
        Guid tenantId,
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}
