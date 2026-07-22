namespace CloudCanvas.Application.Abstractions.Cosmos
{
    public interface IPatchOperation
    {
        string PropertyName { get; set; }
        string PropertyValue { get; set; }
    }
}