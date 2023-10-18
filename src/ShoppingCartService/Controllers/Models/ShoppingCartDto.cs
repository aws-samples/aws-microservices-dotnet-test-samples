using System.ComponentModel.DataAnnotations;

namespace ShoppingCartService.Controllers.Models;

public record CreateShoppingCartDto([Required] ShippingAddressDto ShippingAddress);


public record ShoppingCartDto
{
    public string Id { get; init; } = null!;
    public ShippingAddressDto ShippingAddress { get; init; } = null!;
    public IEnumerable<string> Items { get; init; } = Enumerable.Empty<string>();

    public virtual bool Equals(ShoppingCartDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && ShippingAddress.Equals(other.ShippingAddress) && Items.SequenceEqual(other.Items);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ShippingAddress, Items);
    }
}