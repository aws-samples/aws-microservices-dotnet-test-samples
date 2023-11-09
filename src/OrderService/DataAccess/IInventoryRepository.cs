namespace OrderService.DataAccess;

public interface IInventoryRepository
{
    Task<bool> CheckItemQuantity(string productId, uint quantity);
}