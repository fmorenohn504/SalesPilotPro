using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

public sealed class HttpTenantContext : ITenantContext
{
    private Guid _tenantId;

    public Guid TenantId => _tenantId;

    public void SetTenant(Guid tenantId)
    {
        _tenantId = tenantId;
    }
}
