using SalesPilotPro.Core.Security;

namespace SalesPilotPro.Api.Security;

public sealed class DevSessionValidationClient : ISessionValidationClient
{
    public Task<bool> IsSessionValidAsync(
        Guid tenantId,
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // DEV ONLY: siempre válida
        return Task.FromResult(true);
    }
}
