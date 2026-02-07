using System;

namespace SalesPilotPro.Api.Entities
{
    public sealed class AuditEvent
    {
        public Guid Id { get; set; }

        public DateTime OccurredAtUtc { get; set; }

        public Guid TenantId { get; set; }

        public string ActorUserId { get; set; } = null!;

        public string SessionId { get; set; } = null!;

        public string? CorrelationId { get; set; }

        public string Action { get; set; } = null!;

        public string TargetType { get; set; } = null!;

        public string? TargetId { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }
    }
}
