namespace CloudCanvas.Domain.Abstractions
{
    public interface IHasTimestamps
    {
        public DateTimeOffset GetCreatedOn();
        public DateTimeOffset GetDeletedOn();
        public DateTimeOffset GetModifiedOn();

        public void SetDeletedOn(DateTimeOffset? time = null);
        public void SetModifiedOn(DateTimeOffset? time = null);
    }
}