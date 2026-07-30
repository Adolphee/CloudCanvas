namespace CloudCanvas.Domain.Abstractions
{
    public class AuditableEntity: IHasTimestamps
    {
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset DeletedOn { get; set; } = default!;
        public DateTimeOffset ModifiedOn { get; set; } = default!;

        public DateTimeOffset GetCreatedOn() => CreatedOn;

        public DateTimeOffset GetDeletedOn() => DeletedOn;

        public DateTimeOffset GetModifiedOn() => ModifiedOn;

        public void SetCreatedOn()
        {
            CreatedOn = DateTimeOffset.Now;
        }

        public void SetDeletedOn()
        {
            DeletedOn = DateTimeOffset.Now;
        }

        public void SetModifiedOn()
        {
            ModifiedOn = DateTimeOffset.Now;
        }
    }
}
