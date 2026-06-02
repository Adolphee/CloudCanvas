using CloudCanvas.Shared.DTOs;
using Microsoft.Azure.Amqp.Encoding;
using Microsoft.Azure.Cosmos;
using System.Collections.ObjectModel;

namespace CloudCanvas.Shared.Utilities
{
    public static class PatchOperationBuilder
    {
        // This class basically determines which GalleryItem properties can be modified by the user 
        public static IReadOnlyList<PatchOperation> For(PatchGalleryItemDTO metadata, bool enableSoftDelte = false)
        {
            Validate.Object(metadata);
            var ops = new List<PatchOperation>();
            if (!String.IsNullOrWhiteSpace(metadata.DisplayName))
                ops.Add(PatchOperation.Add($"/displayName", metadata.DisplayName));
            if (!String.IsNullOrWhiteSpace(metadata.GalleryName))
                ops.Add(PatchOperation.Add($"/galleryName", metadata.GalleryName));
            if (!String.IsNullOrWhiteSpace(metadata.Project))
                ops.Add(PatchOperation.Add($"/project", metadata.Project));
            if (!String.IsNullOrWhiteSpace(metadata.Description))
                ops.Add(PatchOperation.Add($"/description", metadata.Description));
            if(metadata.UserTags?.Count > 0) // keep 1 thumbnail until feature to manually generate new ones from GUI
                ops.Add(PatchOperation.Add($"/userTags", metadata.UserTags));
            if (enableSoftDelte && metadata.DeletedOn != null && metadata.DeletedOn > DateTimeOffset.MinValue)
                ops.Add(PatchOperation.Add($"/deletedOn", metadata.DeletedOn));
            return new ReadOnlyCollection<PatchOperation>(ops);
        }
    }
}
