namespace CloudCanvas.Application.Common.Constants
{
    public abstract class ServiceBus
    {
        public const string Uri = "ServiceBusUri";
        public const string ManagedIdentity = "SBManagedIdentity";
        public abstract class Topics
        {
            public const string FileUpdates = "file-updates";
        }

        public abstract class Props
        {
            public const string EventType = "eventType";
            public const string ThumbnailSize = "thumbnailSize";
            public const string ContainerName = "ContainerName";
            public const string Operation = "Operation";
        }

        public abstract class Subs
        {
            public const string ExtractMetaData = "extract-metadata";
            public const string CreateThumbnail = "create-thumbail";
            public const string ResizeImage = "resize-image";
            public const string PersistMetadata = "persist-metadata";
        }

        public abstract class Ops
        {
            public const string Tag = "tag-photo";
            public const string Caption = "caption-photo";
        }

        public abstract class Status
        {
            public const string NewBlobDetected = "New Photo Detected";
            public const string ThumbnailCreated = "Thumbnail Created";
            public const string ImageResized = "Image Resized";
            public const string MetadataPersisted = "Metadata Persisted";
            public const string Intelligence = "intelligence";
            public const string OrchestrationFinished = "Thumbnail Orchestration Concluded. Metadata updated.";
        }
    }

}