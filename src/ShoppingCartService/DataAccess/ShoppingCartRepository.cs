using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.DataAccess.Entities;

namespace ShoppingCartService.DataAccess;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly DynamoDBContext _context;

    public ShoppingCartRepository(IAmazonDynamoDB amazonDynamoDb)
    {
        _context = new DynamoDBContext(amazonDynamoDb);
    }

    public async Task<ShoppingCartDo> CreateAsync(ShippingAddress shippingAddress)
    {
        var shoppingCartDo = new ShoppingCartDo
        {
            Id = Guid.NewGuid().ToString(),
            ShippingAddress = shippingAddress
        };
        await _context.SaveAsync(shoppingCartDo);

        return shoppingCartDo;
    }

    public async Task<ShoppingCartDo?> FindByIdAsync(string shoppingCartId)
    {
        return await _context.LoadAsync<ShoppingCartDo>(shoppingCartId);
    }

    public async Task<ShoppingCartDo?> AddItemToCart(string shoppingCartId, string productId)
    {
        var shoppingCartDo = await _context.LoadAsync<ShoppingCartDo>(shoppingCartId);

        if (shoppingCartDo is null) return null;

        shoppingCartDo.Items.Add(productId);

        await _context.SaveAsync(shoppingCartDo);

        return shoppingCartDo;
    }
}