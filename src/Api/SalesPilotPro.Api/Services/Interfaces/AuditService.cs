using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SalesPilotPro.Api.Entities;
using SalesPilotPro.Api.Persistence;
using SalesPilotPro.Api.Services.Interfaces;

namespace SalesPilotPro.Api.Services
{
    public sealed class AuditService : IAuditService
    {
        private readonly CrmDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(
            CrmDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RecordAsync(
            string action,
            string targetType,
            string? targetId = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            var user = httpContext.User;

            var tenantIdClaim = user.FindFirst("tid")?.Value;
            var userIdClaim = user.FindFirst("sub")?.Value;
            var sessionIdClaim = user.FindFirst("sid")?.Value;

            if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(userIdClaim) ||
                string.IsNullOrWhiteSpace(sessionIdClaim))
            {
                return;
            }

            var auditEvent = new AuditEvent
            {
                Id = Guid.NewGuid(),
                OccurredAtUtc = DateTime.UtcNow,
                TenantId = tenantId,
                ActorUserId = userIdClaim,
                SessionId = sessionIdClaim,
                CorrelationId = httpContext.TraceIdentifier,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString()
            };

            _dbContext.AuditEvents.Add(auditEvent);
            await _dbContext.SaveChangesAsync();
        }
    }
}
