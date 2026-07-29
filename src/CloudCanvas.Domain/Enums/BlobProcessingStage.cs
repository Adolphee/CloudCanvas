namespace CloudCanvas.Domain.Enums
{
    public enum ProcessingStage
    {
        UploadSuccessful,
        ExtractMetadata,
        CreateThumbnail,
        UpdateMetadata,
        Intelligence,
        Completed
    }
}
