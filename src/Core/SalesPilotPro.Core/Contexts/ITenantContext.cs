namespace SalesPilotPro.Core.Contexts;

public interface ITenantContext
{
    Guid? TenantId { get; }
    string? TenantCode { get; }
    bool IsResolved { get; }
}
