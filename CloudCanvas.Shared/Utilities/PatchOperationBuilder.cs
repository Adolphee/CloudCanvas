using CloudCanvas.Shared.DTOs;
using Microsoft.Azure.Cosmos;
using System.Collections.ObjectModel;

namespace CloudCanvas.Shared.Utilities
{
    public static class PatchOperationBuilder
    {
        public static IReadOnlyList<PatchOperation> For(PatchGalleryItemDTO metadata)
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
            return new ReadOnlyCollection<PatchOperation>(ops);
        }
    }
}
