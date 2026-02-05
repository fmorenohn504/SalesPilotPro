using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

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
