using ShoppingCartService.BusinessLogic.Models;

namespace ShoppingCartService.DataAccess;

public interface IOrderServiceNotifications
{
    Task SendOrder(IEnumerable<string> itemIds, ShippingAddress shippingAddress);
}