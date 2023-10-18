namespace ShoppingCartService.BusinessLogic.Models;

public record ShippingAddress
{
    public string Name { get; init; } = null!;
    public string Country { get; init; } = null!;
    public string City { get; init; } = null!;
    public string Street { get; init; } = null!;
}