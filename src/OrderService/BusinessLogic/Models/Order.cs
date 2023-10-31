namespace OrderService.BusinessLogic.Models;

public enum OrderStatus
{
    NoItemsInOrder,
    MissingItems,
    ReadyForShipping
}

public class Order
{
    public Order(string customerName, string shippingAddress, IEnumerable<OrderItem> items)
    {
        Items = items;
        CustomerName = customerName;
        ShippingAddress = shippingAddress;
    }

    private IEnumerable<OrderItem> Items { get; }
    private string CustomerName { get; }
    private string ShippingAddress { get; }

    public OrderStatus Status
    {
        get
        {
            if (!Items.Any())
                return OrderStatus.NoItemsInOrder;

            return Items.Any(i => i.ItemStatus == ItemStatus.NotInInventory)
                ? OrderStatus.MissingItems
                : OrderStatus.ReadyForShipping;
        }
    }

    protected bool Equals(Order other)
    {
        return Items.SequenceEqual(other.Items) && CustomerName == other.CustomerName &&
               ShippingAddress == other.ShippingAddress;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Order)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Items, CustomerName, ShippingAddress);
    }


    public override string ToString()
    {
        return $"{nameof(Items)}: {string.Join("| ", Items.Select(i => i.ToString()))}, " +
               $"{nameof(CustomerName)}: {CustomerName}, " +
               $"{nameof(ShippingAddress)}: {ShippingAddress}";
    }
}