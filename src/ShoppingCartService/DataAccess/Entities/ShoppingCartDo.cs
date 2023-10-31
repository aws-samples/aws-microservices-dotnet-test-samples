using Amazon.DynamoDBv2.DataModel;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.DataAccess.Converters;

namespace ShoppingCartService.DataAccess.Entities;

[DynamoDBTable("ShoppingCarts")]
public class ShoppingCartDo
{
    [DynamoDBHashKey] public string Id { get; init; } = null!;

    [DynamoDBProperty(Converter = typeof(ShippingAddressConverter))]
    public ShippingAddress ShippingAddress { get; init; } = null!;

    public List<string> Items { get; init; } = new();
}