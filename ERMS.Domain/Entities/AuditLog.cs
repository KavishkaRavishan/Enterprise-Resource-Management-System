using System;

namespace ERMS.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public string EntityName { get; private set; } = string.Empty;
        public string EntityId { get; private set; } = string.Empty;
        public string Action { get; private set; } = string.Empty; // Create, Update, Delete
        public DateTime Timestamp { get; private set; }
        public string? OldValues { get; private set; }
        public string? NewValues { get; private set; }
        public Guid? UserId { get; private set; }
        public User? User { get; private set; }

        private AuditLog() { } // EF constructor

        public AuditLog(string entityName, string entityId, string action, string? oldValues, string? newValues, Guid? userId)
        {
            EntityName = entityName;
            EntityId = entityId;
            Action = action;
            Timestamp = DateTime.UtcNow;
            OldValues = oldValues;
            NewValues = newValues;
            UserId = userId;
        }
    }
}
