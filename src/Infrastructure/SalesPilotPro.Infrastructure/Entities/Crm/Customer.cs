using System.ComponentModel.DataAnnotations;
using SalesPilotPro.Infrastructure.Entities;

namespace SalesPilotPro.Infrastructure.Entities.Crm;

public sealed class Customer : TenantEntity
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Auditoría
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedBy { get; set; }
}
