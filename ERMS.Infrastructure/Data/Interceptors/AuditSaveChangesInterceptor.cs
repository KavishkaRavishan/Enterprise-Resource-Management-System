using ERMS.Application.Interfaces;
using ERMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ERMS.Infrastructure.Data.Interceptors
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditSaveChangesInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            AuditChanges(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            AuditChanges(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AuditChanges(DbContext? context)
        {
            if (context == null) return;

            var auditEntries = new List<AuditEntry>();
            var userId = _currentUserService.UserId;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry
                {
                    EntityName = entry.Entity.GetType().Name,
                    UserId = userId,
                    Action = entry.State.ToString()
                };

                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue?.ToString() ?? "";
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            foreach (var auditEntry in auditEntries)
            {
                context.Set<AuditLog>().Add(auditEntry.ToAuditLog());
            }
        }
    }

    internal class AuditEntry
    {
        public string EntityName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object?> OldValues { get; } = new();
        public Dictionary<string, object?> NewValues { get; } = new();

        public AuditLog ToAuditLog()
        {
            var options = new JsonSerializerOptions { WriteIndented = false };
            
            // Format ID nicely as string if simple, or serialize key dictionary
            string entityId = "Unknown";
            if (KeyValues.Count == 1)
            {
                foreach (var val in KeyValues.Values)
                {
                    entityId = val.ToString() ?? "Unknown";
                }
            }
            else if (KeyValues.Count > 1)
            {
                entityId = JsonSerializer.Serialize(KeyValues, options);
            }

            return new AuditLog(
                EntityName,
                entityId,
                Action,
                OldValues.Count > 0 ? JsonSerializer.Serialize(OldValues, options) : null,
                NewValues.Count > 0 ? JsonSerializer.Serialize(NewValues, options) : null,
                UserId
            );
        }
    }
}
