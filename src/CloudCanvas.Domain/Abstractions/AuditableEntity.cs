namespace CloudCanvas.Domain.Abstractions
{
    public abstract class AuditableEntity: IHasTimestamps
    {
        public virtual DateTimeOffset CreatedOn { get; init; } = DateTimeOffset.Now;
        public virtual DateTimeOffset DeletedOn { get; protected set; } = default!;
        public virtual DateTimeOffset ModifiedOn { get; protected set; } = default!;

        public DateTimeOffset GetCreatedOn() => CreatedOn;

        public DateTimeOffset GetDeletedOn() => DeletedOn;

        public DateTimeOffset GetModifiedOn() => ModifiedOn;

        public void SetDeletedOn(DateTimeOffset? time = null)
        {
            DeletedOn = time ?? DateTimeOffset.Now;
        }

        public void SetModifiedOn(DateTimeOffset? time = null)
        {
            ModifiedOn = time ?? DateTimeOffset.Now;
        }
    }
}
