using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

// ⚠️ SOLO DEV – se elimina cuando JWT esté activo
public sealed class DevTenantContext : ITenantContext
{
    public Guid TenantId => Guid.Empty;
    public string TenantCode => "DEV";
}
