using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Infrastructure.Contexts;

// Tenant activo (scoped por request)
public sealed class TenantContext : ITenantContext
{
    public Guid TenantId { get; }
    public string TenantCode { get; }

    public TenantContext(Guid tenantId, string tenantCode)
    {
        TenantId = tenantId;
        TenantCode = tenantCode;
    }
}
