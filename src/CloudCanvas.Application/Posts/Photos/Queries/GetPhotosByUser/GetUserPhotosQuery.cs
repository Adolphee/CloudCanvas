namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosQuery: IRequest<GetUserPhotosResult>
    {
        public string UserId { get; set; }
        public string ContainerName { get; set; }

        public GetUserPhotosQuery(string id, string container = Projection.Containers.UserPhotos)
        {
            UserId = id;
            ContainerName = container;
        }
    }
}
