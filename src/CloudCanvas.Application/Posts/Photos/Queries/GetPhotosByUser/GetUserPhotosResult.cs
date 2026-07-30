using CloudCanvas.Application.Posts.Photos.Queries.GetAllPhotos;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosResult : GetAllPhotosResult
    {
        public GetUserPhotosResult(List<PhotoDTO> posts) : base(posts) { }
    }
}
