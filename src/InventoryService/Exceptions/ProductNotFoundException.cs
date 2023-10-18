using System.Runtime.Serialization;

namespace InventoryService.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException()
    {
    }

    protected ProductNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ProductNotFoundException(string message) : base(message)
    {
    }

    public ProductNotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
