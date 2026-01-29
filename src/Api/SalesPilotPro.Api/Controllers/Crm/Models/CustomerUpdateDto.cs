using System.ComponentModel.DataAnnotations;

namespace SalesPilotPro.Api.Controllers.Crm.Models;

public sealed class CustomerUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
