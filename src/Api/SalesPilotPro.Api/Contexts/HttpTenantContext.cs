using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

public sealed class HttpTenantContext : ITenantContext
{
    public Guid TenantId { get; }

    public string TenantCode { get; }

    public HttpTenantContext(Guid tenantId, string tenantCode)
    {
        TenantId = tenantId;
        TenantCode = tenantCode;
    }
}
