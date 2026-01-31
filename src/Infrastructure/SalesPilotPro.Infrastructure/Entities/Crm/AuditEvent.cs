using System.ComponentModel.DataAnnotations;

namespace SalesPilotPro.Infrastructure.Entities.Crm;

public sealed class AuditEvent
{
    public Guid Id { get; set; }

    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public Guid ActorUserId { get; set; }

    [Required]
    public Guid SessionId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TargetType { get; set; }

    public Guid? TargetId { get; set; }

    [Required]
    public DateTime OccurredAtUtc { get; set; }

    [Required]
    [MaxLength(50)]
    public string CorrelationId { get; set; } = string.Empty;

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [MaxLength(200)]
    public string? UserAgent { get; set; }
}
