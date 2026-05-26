using ERMS.Domain.Enums;

namespace ERMS.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid RecipientId { get; private set; }
        public User Recipient { get; private set; } = null!;
        public string Message { get; private set; } = string.Empty;
        public bool IsRead { get; private set; }
        public NotificationType Type { get; private set; }
        public Guid? ReferenceId { get; private set; }

        private Notification() { } // EF constructor

        public Notification(Guid recipientId, string message, NotificationType type, Guid? referenceId = null)
        {
            RecipientId = recipientId;
            Message = message;
            Type = type;
            ReferenceId = referenceId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            Updated = DateTime.UtcNow;
        }
    }
}
