namespace CloudCanvas.Application.Common.Constants
{
    
    public abstract class BStorage
    {
        public const string Self = "AzureBlobStorage";
        public const string Uri = "BlobStorageUri";
        public const string ManagedIdentity = "BSManagedIdentity";
        public const string BSConnection = "BSConnectionString";

        public abstract class Meta
        {
            public const string Identifier = "identifier";
            public const string OriginalFilename = "originalFilename";
            public static string UploadedBy = "uploadedBy";
            public static string CompletedOn = "completedOn";
            public static string CreatedOn = "createdOn";
            public const string DeletedOn = "deletedOn";
            public const string Container = "container";

        }

        public abstract class Containers
        {
            public const string PhotoGallery = "photogallery";
            public const string Uploads = "uploads";
            public const string ImgConversions = "imgconversions";
            public const string Thumbnails = "thumbnails";
        }
    }

}