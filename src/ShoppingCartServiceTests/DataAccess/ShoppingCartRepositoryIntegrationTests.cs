using Amazon.DynamoDBv2.DataModel;
using Common.TestUtils.TestBaseClasses;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;

namespace ShoppingCartServiceTests.DataAccess;

[TestFixture]
public class ShoppingCartRepositoryIntegrationTests : DynamoDbTestBase
{
    [Test]
    public async Task Create_ReturnSavedDocument()
    {
        var target = new ShoppingCartRepository(Client);

        var address = new ShippingAddress
        {
            Name = "Customer name",
            Country = "USA",
            City = "New York",
            Street = "12345 St."
        };

        var result = await target.CreateAsync(address);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.Not.Null);
            Assert.That(result.ShippingAddress, Is.EqualTo(address));
        });
    }

    [Test]
    public async Task Create_SaveNewShoppingCartInDb()
    {
        var target = new ShoppingCartRepository(Client);

        var address = new ShippingAddress
        {
            Name = "Customer name",
            Country = "USA",
            City = "New York",
            Street = "12345 St."
        };

        var result = await target.CreateAsync(address);

        var context = new DynamoDBContext(Client);
        var actual = await context.LoadAsync<ShoppingCartDo>(result.Id);

        Assert.That(actual.ShippingAddress, Is.EqualTo(address));
    }

    [Test]
    public async Task FindById_ShoppingCartNotInDb_ReturnNull()
    {
        var target = new ShoppingCartRepository(Client);

        var result = await target.FindByIdAsync("cart-1");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindById_ShoppingCarInDb_ReturnCart()
    {
        var target = new ShoppingCartRepository(Client);
        var address = new ShippingAddress
        {
            Name = "Customer name",
            Country = "USA",
            City = "New York",
            Street = "12345 St."
        };

        var createdShoppingCart = await target.CreateAsync(address);
        var shoppingCartId = createdShoppingCart.Id;

        var result = await target.FindByIdAsync(shoppingCartId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ShippingAddress, Is.EqualTo(address));
    }

    [Test]
    public async Task AddItemToCustomer_ItemAddedToDb()
    {
        var target = new ShoppingCartRepository(Client);

        var address = new ShippingAddress
        {
            Name = "Customer name",
            Country = "USA",
            City = "New York",
            Street = "12345 St."
        };

        var shoppingCartDo = await target.CreateAsync(address);
        await target.AddItemToCart(shoppingCartDo.Id, "product-1");

        var context = new DynamoDBContext(Client);
        var actual = await context.LoadAsync<ShoppingCartDo>(shoppingCartDo.Id);

        Assert.That(actual.Items, Is.EqualTo(new[] { "product-1" }));
    }

    [Test]
    public async Task AddItemToCustomer_ShoppingCartReturned()
    {
        var target = new ShoppingCartRepository(Client);

        var address = new ShippingAddress
        {
            Name = "Customer name",
            Country = "USA",
            City = "New York",
            Street = "12345 St."
        };

        var shoppingCartDo = await target.CreateAsync(address);
        var actual = await target.AddItemToCart(shoppingCartDo.Id, "product-1");

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Items, Is.EqualTo(new[] { "product-1" }));
    }

    [Test]
    public async Task AddItemToCustomer_CartNotFound_returnNull()
    {
        var target = new ShoppingCartRepository(Client);
        var result = await target.AddItemToCart("cart-1", "product-1");

        Assert.That(result, Is.Null);
    }
}