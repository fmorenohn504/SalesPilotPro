using Microsoft.EntityFrameworkCore;
using SalesPilotPro.Api.Entities;

namespace SalesPilotPro.Api.Persistence
{
    public sealed class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEvent>(entity =>
            {
                entity.ToTable("AuditEvents", "crm");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.OccurredAtUtc).IsRequired();
                entity.Property(e => e.TenantId).IsRequired();

                entity.Property(e => e.ActorUserId)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.SessionId)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.CorrelationId)
                    .HasMaxLength(100);

                entity.Property(e => e.Action)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.TargetType)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.TargetId)
                    .HasMaxLength(100);

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(45);

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(256);
            });
        }
    }
}
