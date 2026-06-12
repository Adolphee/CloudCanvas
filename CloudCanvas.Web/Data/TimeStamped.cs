using CloudCanvas.Web.Interfaces;

namespace CloudCanvas.Web.Data
{
    public abstract class TimeStamped: IHasTimestamps
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset DeletedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

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
