namespace CloudCanvas.Functions.DTOs
{
    /// <summary>
    /// Represents a message containing metadata extraction information.
    /// </summary>
    /// <remarks>This DTO is used to encapsulate metadata extraction details, including the payload containing
    /// metadata information, for communication purposes.</remarks>
    public class ExtractMetadataMessageDTO: CloudCanvasMessageDTO
    {
        public BlobMetaDTO Payload { get; set; }
    }
}
