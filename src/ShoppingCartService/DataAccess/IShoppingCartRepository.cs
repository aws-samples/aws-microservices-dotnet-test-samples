using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.DataAccess.Entities;

namespace ShoppingCartService.DataAccess;

public interface IShoppingCartRepository
{
    Task<ShoppingCartDo> CreateAsync(ShippingAddress shippingAddress);
    Task<ShoppingCartDo?> FindByIdAsync(string shoppingCartId);
    Task<ShoppingCartDo?> AddItemToCart(string shoppingCartId, string productId);
}