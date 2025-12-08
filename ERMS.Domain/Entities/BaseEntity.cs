namespace ERMS.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime Created { get; protected set; }
        public DateTime Updated { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }
    }
}
