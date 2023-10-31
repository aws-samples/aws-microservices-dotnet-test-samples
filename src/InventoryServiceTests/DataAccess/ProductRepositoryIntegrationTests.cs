using Common.TestUtils.TestBaseClasses;
using InventoryService.Config;
using InventoryService.DataAccess;
using InventoryService.DataAccess.Entities;

namespace InventoryServiceTests.DataAccess;

[TestFixture]
public class ProductRepositoryIntegrationTests : MongoDbTestBase
{
    private const string InvalidId = "507f191e810c19729de860ea";

    private IInventoryDatabaseSettings GetDatabaseSettings()
    {
        return new InventoryDatabaseSettings
        {
            DatabaseName = TestDatabaseName,
            ConnectionString = ConnectionString
        };
    }

    [Test]
    public void FindAll_NoProductInDB_ReturnEmptyList()
    {
        var target = new ProductRepository(GetDatabaseSettings());

        var actual = target.FindAll();

        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void FindAll_HasTwoProductsInDB_ReturnAllProducts()
    {
        var target = new ProductRepository(GetDatabaseSettings());

        var product1 = new Product { Name = "name-1", Quantity = 1, Price = 10 };
        target.Create(product1);

        var product2 = new Product { Name = "name-2", Quantity = 2, Price = 20 };
        target.Create(product2);

        var actual = target.FindAll().ToList();

        var expected = new List<Product> { product1, product2 };
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetById_hasThreeCartsInDB_returnReturnOnlyCartWithCorrectId()
    {
        var target = new ProductRepository(GetDatabaseSettings());

        var product1 = new Product { Name = "name-1", Quantity = 1, Price = 10 };
        target.Create(product1);

        var product2 = new Product { Name = "name-2", Quantity = 2, Price = 20 };
        target.Create(product2);

        var product3 = new Product { Name = "name-3", Quantity = 3, Price = 30 };
        target.Create(product3);

        var actual = target.FindById(product2.Id);

        Assert.That(actual, Is.EqualTo(product2));
    }

    [Test]
    public void GetById_CartNotFound_ReturnNull()
    {
        var target = new ProductRepository(GetDatabaseSettings());

        var product1 = new Product { Name = "name-1", Quantity = 1, Price = 10 };
        target.Create(product1);

        var product2 = new Product { Name = "name-2", Quantity = 2, Price = 20 };
        target.Create(product2);

        var product3 = new Product { Name = "name-3", Quantity = 3, Price = 30 };
        target.Create(product3);

        var actual = target.FindById(InvalidId);

        Assert.That(actual, Is.Null);
    }

    [Test]
    public void Update_CartNotFound_DoNotFail()
    {
        var target = new ProductRepository(GetDatabaseSettings());
        var product1 = new Product { Name = "name-1", Quantity = 1, Price = 10 };

        target.Update(InvalidId, product1);
    }

    [Test]
    public void Update_CartFound_UpdateValue()
    {
        var target = new ProductRepository(GetDatabaseSettings());
        var product1 = new Product { Name = "name-1", Quantity = 1, Price = 10 };
        target.Create(product1);
        product1.Quantity = 100;

        target.Update(product1.Id, product1);

        target.FindById(product1.Id);

        Assert.That(product1.Quantity, Is.EqualTo(100));
    }
}