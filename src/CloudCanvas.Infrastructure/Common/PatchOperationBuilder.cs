using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Galleries;
using CloudCanvas.Application.Posts.Photos;
using Microsoft.Azure.Cosmos;
using System.Collections.ObjectModel;

namespace CloudCanvas.Infrastructure.Common
{
    public static class PatchOperationBuilder
    {
        // This class basically determines which GalleryItem Properties can be modified by the user 
        public static IReadOnlyList<PatchOperation> For(PhotoDTO photo, bool enableSoftDelte = true)
        {
            var ops = new List<PatchOperation>();
            if (!String.IsNullOrWhiteSpace(photo.Title))
                ops.Add(PatchOperation.Add($"/title", photo.Title));
            if (!String.IsNullOrWhiteSpace(photo.GalleryId))
                ops.Add(PatchOperation.Add($"/galleryId", photo.GalleryId));
            if (!String.IsNullOrWhiteSpace(photo.Description))
                ops.Add(PatchOperation.Add($"/description", photo.Description));
            if(photo.UserTags?.Count > 0)
                ops.Add(PatchOperation.Add($"/userTags", photo.UserTags));
            return new ReadOnlyCollection<PatchOperation>(ops);
        }

        public static IReadOnlyList<PatchOperation> For(GalleryDTO gallery, bool enableSoftDelte = true)
        {
            var ops = new List<PatchOperation>();
            if (!String.IsNullOrWhiteSpace(gallery.DisplayName))
                ops.Add(PatchOperation.Add($"/displayName", gallery.DisplayName));
            if (!String.IsNullOrWhiteSpace(gallery.Description))
                ops.Add(PatchOperation.Add($"/description", gallery.Description));
            if(gallery.UserTags?.Count > 0)
                ops.Add(PatchOperation.Add($"/userTags", gallery.UserTags));
            return new ReadOnlyCollection<PatchOperation>(ops);
        }

        public static IReadOnlyList<PatchOperation> ForSoftDelete(PostDTO photo)
        {
            var deleted = photo.TimeStamps?.DeletedOn != null && photo.TimeStamps.DeletedOn > DateTimeOffset.MinValue;
            var operation = PatchOperation.Add($"/timeStamps/deletedOn", deleted ? photo.TimeStamps!.DeletedOn : DateTimeOffset.Now);
            return new ReadOnlyCollection<PatchOperation>(new List<PatchOperation> { operation });
        }
    }
}
