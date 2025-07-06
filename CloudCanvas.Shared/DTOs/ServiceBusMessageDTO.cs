namespace CloudCanvas.Shared.DTOs
{
    /// <summary>
    /// Represents a data transfer object (DTO) for a message in the CloudCanvas system.
    /// </summary>
    /// <remarks>This class encapsulates the key details of a CloudCanvas message, including the event type
    /// and subject. It is typically used to transfer message data between components or services.</remarks>
    public class ServiceBusMessageDTO
    {
        public string? Event { get; set; }
        public string? Subject { get; set; }
    }
}
