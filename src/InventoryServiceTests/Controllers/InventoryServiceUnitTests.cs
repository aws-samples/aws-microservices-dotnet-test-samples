using Common.TestUtils;
using InventoryService.Contracts.Models;
using InventoryService.Controllers;
using InventoryService.DataAccess;
using InventoryService.DataAccess.Entities;
using InventoryService.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace InventoryServiceTests.Controllers;

[TestFixture]
public class InventoryServiceUnitTests : TestBase<MappingProfile>
{
    [Test]
    public void AddProductToInventory_ValidData_SaveProductInDb()
    {
        var fakeRepository = AutoFake.Resolve<IProductRepository>();
        A.CallTo(() => fakeRepository.Create(A<Product>._))
            .Returns(new Product { Id = "product-1" });

        var target = AutoFake.Resolve<InventoryController>();

        var result = target.AddToInventory(new CreateProductDto("product name", 1));

        Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());
        var cartId = ((CreatedAtRouteResult)result.Result!)?.RouteValues?["id"]?.ToString();
        Assert.That(cartId, Is.EqualTo("product-1"));
    }

    [Test]
    public void FindById_ProductNotFound_RreturnNull()
    {
        var fakeInventoryRepository = AutoFake.Resolve<IProductRepository>();
        A.CallTo(() => fakeInventoryRepository.FindById("product-1")).Returns(null);

        var target = AutoFake.Resolve<InventoryController>();

        var result = target.FindById("product-1");

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void FindById_ProductFound_ReturnProduct()
    {
        const string productName = "product name";
        const string productId = "product-1";


        var fakeInventoryRepository = AutoFake.Resolve<IProductRepository>();

        A.CallTo(() => fakeInventoryRepository.FindById(A<string>._)).Returns(new Product
        {
            Id = productId,
            Name = productName,
            Price = 10.00,
            Quantity = 1
        });

        var target = AutoFake.Resolve<InventoryController>();

        var result = target.FindById(productId);

        var expected = new ProductDto(productId, productName, 10, 1);

        Assert.That(result.Value, Is.EqualTo(expected));
    }

    [Test]
    public void GetAll_NoProductsInDB_ReturnEmptyEnumeration()
    {
        var fakeInventoryRepository = AutoFake.Resolve<IProductRepository>();
        A.CallTo(() => fakeInventoryRepository.FindAll()).Returns(Enumerable.Empty<Product>());

        var target = AutoFake.Resolve<InventoryController>();

        var result = target.GetAll();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetAll_ProductsInDB_ReturnProducts()
    {
        var fakeInventoryRepository = AutoFake.Resolve<IProductRepository>();
        A.CallTo(() => fakeInventoryRepository.FindAll()).Returns(new List<Product>
        {
            new Product { Id = "product-1", Name = "product 1", Quantity = 1, Price = 10},
            new Product { Id = "product-2", Name = "product 2", Quantity = 2, Price = 20}
        });

        var target = AutoFake.Resolve<InventoryController>();

        var result = target.GetAll();

        var expected = new List<ProductDto>
        {
            new("product-1", "product 1", 10,1),
            new("product-2", "product 2", 20, 2)
        };

        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void UpdateQuantity_ProductNotFound_ReturnNotFound()
    {
        var fakeInventoryRepository = AutoFake.Resolve<IProductRepository>();
        A.CallTo(() => fakeInventoryRepository.FindById("product-1")).Returns(null);

        var target = AutoFake.Resolve<InventoryController>();
        var response = target.UpdateQuantity("product-1", 2);

        Assert.That(response.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void UpdateQuantity_ProductFound_SaveUpdateProductInDB()
    {
        var fakeInventoryRepository = AutoFake.Resolve<IProductRepository>();
        A.CallTo(() => fakeInventoryRepository.FindById("product-1"))
            .Returns(new Product { Id = "product-1", Name = "name", Quantity = 1, Price = 10 });

        var target = AutoFake.Resolve<InventoryController>();
        target.UpdateQuantity("product-1", 2);

        var expected = new Product { Id = "product-1", Name = "name", Quantity = 2, Price = 10};

        A.CallTo(() => fakeInventoryRepository.Update("product-1", expected))
            .MustHaveHappened();
    }

    [Test]
    public void UpdateQuantity_ProductFound_ReturnUpdatedProduct()
    {
        var fakeInventoryRepository = AutoFake.Resolve<IProductRepository>();
        A.CallTo(() => fakeInventoryRepository.FindById("product-1"))
            .Returns(new Product { Id = "product-1", Name = "name", Quantity = 1, Price = 10});

        var target = AutoFake.Resolve<InventoryController>();
        var result = target.UpdateQuantity("product-1", 2);

        var expected = new ProductDto("product-1", "name", 10, 2);

        Assert.That(result.Value, Is.EqualTo(expected));
    }
}