namespace SalesPilotPro.Core.Security;

public interface ISessionValidationClient
{
    Task<bool> IsSessionValidAsync(
        Guid tenantId,
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken);
}
