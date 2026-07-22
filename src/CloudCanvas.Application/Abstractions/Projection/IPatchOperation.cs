namespace CloudCanvas.Application.Abstractions.Projection
{
    public interface IPatchOperation
    {
        string PropertyName { get; set; }
        string PropertyValue { get; set; }
    }
}