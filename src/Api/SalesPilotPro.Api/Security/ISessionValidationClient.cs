namespace SalesPilotPro.Api.Security;

public interface ISessionValidationClient
{
    Task<bool> ValidateAsync(
        Guid tenantId,
        Guid sessionId,
        Guid userId,
        CancellationToken ct);
}
