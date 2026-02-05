using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

public sealed class DevTenantContext : ITenantContext
{
    public Guid TenantId { get; private set; }
    public string TenantCode { get; private set; } = "DEV";

    public void SetTenant(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
