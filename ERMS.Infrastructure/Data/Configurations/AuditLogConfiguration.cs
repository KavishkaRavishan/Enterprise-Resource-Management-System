using ERMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERMS.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.EntityName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Timestamp)
                .IsRequired();

            builder.Property(a => a.OldValues)
                .IsRequired(false);

            builder.Property(a => a.NewValues)
                .IsRequired(false);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
