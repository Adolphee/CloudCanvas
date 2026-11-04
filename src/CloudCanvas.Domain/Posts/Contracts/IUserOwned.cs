namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IUserOwned: IEntity
    {
        public abstract string UserId { get; init; }
    }
}