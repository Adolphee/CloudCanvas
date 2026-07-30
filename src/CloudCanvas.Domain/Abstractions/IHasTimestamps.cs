namespace CloudCanvas.Domain.Abstractions
{
    public interface IHasTimestamps
    {
        public DateTimeOffset GetCreatedOn();
        public DateTimeOffset GetDeletedOn();
        public DateTimeOffset GetModifiedOn();

        public void SetCreatedOn();
        public void SetDeletedOn();
        public void SetModifiedOn();
    }
}