using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Controllers;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Mapping;

namespace ShoppingCartServiceTests.Controllers;

[TestFixture]
public class ShoppingCartControllerUnitTests : TestBase<MappingProfile>
{
    [Test]
    public async Task Create_ReturnCartIdAndLink()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();
        A.CallTo(() => fakeShoppingCartRepository.CreateAsync(A<ShippingAddress>._))
            .Returns(new ShoppingCartDo { Id = "cart-1" });

        var shippingAddress = new ShippingAddressDto("customer-1", "country-1", "city-1", "street-1");
        var createShoppingCart = new CreateShoppingCartDto(shippingAddress);

        var target = AutoFake.Resolve<ShoppingCartController>();
        var result = await target.Create(createShoppingCart);

        Assert.That(result.Result, Is.TypeOf<CreatedAtRouteResult>());
        var createdAtRouteResult = (CreatedAtRouteResult)result.Result!;
        var cartId = createdAtRouteResult.RouteValues!["id"]!.ToString();
        Assert.That(cartId, Is.EqualTo("cart-1"));
    }

    [Test]
    public async Task Create_ReturnCreatedCartAndTotalCostEqualsZero()
    {
        const string cartId = "cart-1";

        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();

        A.CallTo(() => fakeShoppingCartRepository.CreateAsync(A<ShippingAddress>._))
            .Returns(new ShoppingCartDo
            {
                Id = cartId,
                ShippingAddress = new ShippingAddress
                {
                    Name = "customer-1", Country = "country-1", City = "city-1", Street = "street-1"
                }
            });

        var shippingAddress = new ShippingAddressDto("customer-1", "country-1", "city-1", "street-1");
        var createShoppingCart = new CreateShoppingCartDto(shippingAddress);

        var target = AutoFake.Resolve<ShoppingCartController>();
        var result = await target.Create(createShoppingCart);

        var createdAtRouteResult = (CreatedAtRouteResult)result.Result!;

        var actual = createdAtRouteResult.Value;

        Assert.That(actual, Is.Not.Null);

        var expected = new ShoppingCartDto
        {
            Id = cartId,
            ShippingAddress = shippingAddress,
            Items = new List<string>()
        };

        Assert.That(actual, Is.EqualTo(expected));
    }

[Test]
public async Task FindById_CartNotFound_ReturnNotFound()
{
    var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();
    A.CallTo(() => fakeShoppingCartRepository.FindByIdAsync(A<string>._))
        .Returns(Task.FromResult<ShoppingCartDo?>(null));

    var target = AutoFake.Resolve<ShoppingCartController>();

    var actual = await target.FindById("cart-1");

    Assert.That(actual.Result, Is.TypeOf<NotFoundResult>());
}

    [Test]
    public async Task FindById_CartFound_ReturnCart()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();
        A.CallTo(() => fakeShoppingCartRepository.FindByIdAsync(A<string>._)).Returns(new ShoppingCartDo
        {
            Id = "cart-1",
            ShippingAddress = new ShippingAddress
            {
                Name = "customer-1",
                Country = "country-1",
                City = "city-1",
                Street = "street-1"
            },
            Items = new List<string>
            {
                "product-1", "product-2"
            }
        });

        var target = AutoFake.Resolve<ShoppingCartController>();

        var actual = await target.FindById("cart-1");

        var result = actual.Value;

        var expected = new ShoppingCartDto
        {
            Id = "cart-1",
            ShippingAddress = new ShippingAddressDto("customer-1", "country-1", "city-1", "street-1"),
            Items = new List<string> { "product-1", "product-2" }
        };

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task AddItemToCart_CartNotFound_ReturnBadRequest()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();

        A.CallTo(() => fakeShoppingCartRepository
                .AddItemToCart(A<string>._, A<string>._))
            .Returns(Task.FromResult<ShoppingCartDo?>(null));

        var fakeInventoryRepository = AutoFake.Resolve<IInventoryRepository>();
        A.CallTo(() => fakeInventoryRepository.ProductExist("product-1"))
            .Returns(true);

        var target = AutoFake.Resolve<ShoppingCartController>();

        var result = await target.AddItemToCart("cart-1", "product-1");

        Assert.That(result.Result, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task AddItemToCart_CartFoundItemNotInInventory_ReturnNotFound()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();

        A.CallTo(() => fakeShoppingCartRepository
            .AddItemToCart(A<string>._, A<string>._)).Returns(
            new ShoppingCartDo
            {
                Id = "cart-1",
                ShippingAddress = new ShippingAddress
                {
                    Name = "customer-1",
                    Country = "country-1",
                    City = "city-1",
                    Street = "street-1"
                }
            });

        var fakeInventoryRepository = AutoFake.Resolve<IInventoryRepository>();
        A.CallTo(() => fakeInventoryRepository.ProductExist("product-1"))
            .Returns(false);

        var target = AutoFake.Resolve<ShoppingCartController>();

        var result = await target.AddItemToCart("cart-1", "product-1");

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task AddItemToCart_CartFoundItemInInventory_AddItemToCart()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();

        var fakeInventoryRepository = AutoFake.Resolve<IInventoryRepository>();
        A.CallTo(() => fakeInventoryRepository.ProductExist("product-1"))
            .Returns(true);

        var target = AutoFake.Resolve<ShoppingCartController>();

        var result = await target.AddItemToCart("cart-1", "product-1");

        Assert.Multiple(
            () =>
            {
                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                A.CallTo(() => fakeShoppingCartRepository
                        .AddItemToCart("cart-1", "product-1"))
                    .MustHaveHappened();
            });
    }

    [Test]
    public async Task Checkout_ShoppingCartNotFound_ReturnNotFound()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();

        A.CallTo(() => fakeShoppingCartRepository.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<ShoppingCartDo?>(null));

        var target = AutoFake.Resolve<ShoppingCartController>();

        var result = await target.Checkout("cart-1");

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Checkout_ShoppingCartFound_ReturnOk()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();

        A.CallTo(() => fakeShoppingCartRepository.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<ShoppingCartDo?>(new ShoppingCartDo { Items = new List<string> { "item-1" } }));

        var target = AutoFake.Resolve<ShoppingCartController>();

        var result = await target.Checkout("cart-1");

        Assert.That(result, Is.TypeOf<OkResult>());
    }

    [Test]
    public async Task Checkout_ShoppingCartFound_SendDataInQueue()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();
        var fakeOrderServiceNotifications = AutoFake.Resolve<IOrderServiceNotifications>();

        var shippingAddress = new ShippingAddress
        {
            Name = "Customer name",
            Country = "Country-1",
            City = "City-1",
            Street = "Street-1"
        };

        A.CallTo(() => fakeShoppingCartRepository.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<ShoppingCartDo?>(new ShoppingCartDo
            {
                Id = "cart-1",
                Items = new List<string> { "item-1", "item-2" },
                ShippingAddress = shippingAddress
            }));

        var target = AutoFake.Resolve<ShoppingCartController>();

        await target.Checkout("cart-1");

        A.CallTo(() =>
                fakeOrderServiceNotifications.SendOrder(
                    A<IEnumerable<string>>
                        .That.IsSameSequenceAs(new[] { "item-1", "item-2" }),
                    shippingAddress))
            .MustHaveHappened();
    }

    [Test]
    public async Task SendOrder_ShoppingCartDoNotHaveAnyItems_ReturnError()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();

        var shippingAddress = new ShippingAddress
        {
            Name = "Customer name",
            Country = "Country-1",
            City = "City-1",
            Street = "Street-1"
        };

        A.CallTo(() => fakeShoppingCartRepository.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<ShoppingCartDo?>(new ShoppingCartDo
            {
                Id = "cart-1",
                Items = new List<string>(),
                ShippingAddress = shippingAddress
            }));

        var target = AutoFake.Resolve<ShoppingCartController>();

        var result = await target.Checkout("cart-1");

        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task SendOrder_OrderProcessingNotificationThrowsException_ReturnError()
    {
        var fakeShoppingCartRepository = AutoFake.Resolve<IShoppingCartRepository>();
        var fakeOrderServiceNotifications = AutoFake.Resolve<IOrderServiceNotifications>();

        var shippingAddress = new ShippingAddress
        {
            Name = "Customer name",
            Country = "Country-1",
            City = "City-1",
            Street = "Street-1"
        };

        A.CallTo(() => fakeOrderServiceNotifications.SendOrder(A<IEnumerable<string>>._, A<ShippingAddress>._))
            .Throws<ApplicationException>();

        A.CallTo(() => fakeShoppingCartRepository.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<ShoppingCartDo?>(new ShoppingCartDo
            {
                Id = "cart-1",
                Items = new List<string> { "item-1" },
                ShippingAddress = shippingAddress
            }));

        var target = AutoFake.Resolve<ShoppingCartController>();

        var result = await target.Checkout("cart-1");

        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }
}