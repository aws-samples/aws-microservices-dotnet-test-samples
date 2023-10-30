using OrderService.DataAccess.Entities;

namespace OrderService.DataAccess;

public interface IInventoryRepository
{
    Task<GetFromInventoryResult> GetItemFromInventory(string productId);
}