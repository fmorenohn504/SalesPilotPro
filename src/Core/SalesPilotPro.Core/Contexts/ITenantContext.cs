namespace SalesPilotPro.Core.Contexts;

public interface ITenantContext
{
    Guid TenantId { get; }
}
