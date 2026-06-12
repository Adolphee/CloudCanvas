namespace CloudCanvas.Web.Interfaces
{
    public interface IHasTimestamps
    {        
        DateTimeOffset CreatedOn { get; set; }
        DateTimeOffset DeletedOn { get; set; }
        DateTimeOffset ModifiedOn { get; set; }


        public void SetCreatedOn();
        public void SetDeletedOn();
        public void SetModifiedOn();
    }
}