using InventoryService.DataAccess.Entities;

namespace InventoryService.DataAccess;

public interface IProductRepository
{
    Product Create(Product product);
    Product? FindById(string id);
    IEnumerable<Product> FindAll();
    void Update(string id, Product product);
}