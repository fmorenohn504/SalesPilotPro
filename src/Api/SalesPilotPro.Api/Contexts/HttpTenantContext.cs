using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

public sealed class HttpTenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public string? TenantCode { get; private set; }

    public bool IsResolved => TenantId.HasValue;

    public void SetTenant(Guid tenantId, string tenantCode)
    {
        TenantId = tenantId;
        TenantCode = tenantCode;
    }
}
