using ERMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERMS.Infrastructure.Data.Configurations
{
    public class TimeLogConfiguration : IEntityTypeConfiguration<TimeLog>
    {
        public void Configure(EntityTypeBuilder<TimeLog> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.HoursSpent)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.DateLogged)
                .IsRequired();

            builder.HasOne(t => t.Task)
                .WithMany(tk => tk.TimeLogs)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.User)
                .WithMany(u => u.TimeLogs)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
