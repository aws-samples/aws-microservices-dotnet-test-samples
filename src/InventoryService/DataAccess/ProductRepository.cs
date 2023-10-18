using InventoryService.Config;
using InventoryService.DataAccess.Entities;
using MongoDB.Driver;

namespace InventoryService.DataAccess;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IInventoryDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _products = database.GetCollection<Product>("Products");
    }

    public Product Create(Product product)
    {
        _products.InsertOne(product);

        return product;
    }

    public Product FindById(string id)
    {
        return _products.Find(product => product.Id == id)
            .FirstOrDefault();
    }

    public IEnumerable<Product> FindAll()
    {
        return _products.Find(_ => true).ToEnumerable();
    }

    public void Update(string id, Product product)
    {
        _products.ReplaceOne(p => p.Id == id, product);
    }
}