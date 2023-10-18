namespace ShoppingCartService.DataAccess;

public interface IInventoryRepository
{
    Task<bool> ProductExist(string productId);
}