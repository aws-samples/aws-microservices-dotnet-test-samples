namespace OrderService.Contracts;

public class CreateOrderMessage
{
    public string Id { get; } 
    public IEnumerable<string> Items { get; set; } = Enumerable.Empty<string>();

    public string CustomerName { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public CreateOrderMessage(string id)
    {
        Id = id;
    }

    protected bool Equals(CreateOrderMessage other)
    {
        return Items.SequenceEqual(other.Items) && CustomerName == other.CustomerName &&
               ShippingAddress == other.ShippingAddress;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CreateOrderMessage)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Items, CustomerName, ShippingAddress);
    }

    public override string ToString()
    {
        return
            $"{nameof(Items)}: {Items}, {nameof(CustomerName)}: {CustomerName}, {nameof(ShippingAddress)}: {ShippingAddress}";
    }
}