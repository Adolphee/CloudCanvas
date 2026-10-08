
namespace CloudCanvas.Application.Common.Constants
{
    public abstract class Projection
    {
        public const string Sql = "cloudcosmos_sql";
        public const string Uri = "CosmosEndpointURI";
        
        public abstract class Containers {
            public const string UserPhotos = "user_photos";
            public const string Galleries = "galleries";
            public const string Comments = "comments";
            public const string Users = "users";
        }
    }
}