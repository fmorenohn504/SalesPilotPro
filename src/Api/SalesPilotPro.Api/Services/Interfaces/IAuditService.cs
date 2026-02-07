using System.Threading.Tasks;

namespace SalesPilotPro.Api.Services.Interfaces
{
    public interface IAuditService
    {
        Task RecordAsync(
            string action,
            string targetType,
            string? targetId = null
        );
    }
}
