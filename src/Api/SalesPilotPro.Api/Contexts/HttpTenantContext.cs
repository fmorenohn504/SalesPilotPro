using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

public sealed class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.Items["TenantId"];
            return value is Guid id
                ? id
                : throw new InvalidOperationException("TenantId not available in context");
        }
    }
}
