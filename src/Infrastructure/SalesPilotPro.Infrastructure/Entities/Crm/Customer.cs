using System.ComponentModel.DataAnnotations;

namespace SalesPilotPro.Infrastructure.Entities.Crm;

public sealed class Customer
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
