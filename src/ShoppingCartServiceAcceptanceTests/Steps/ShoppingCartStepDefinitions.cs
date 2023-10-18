using System.Net;
using Amazon.SQS;
using Common.TestUtils.Extensions;
using InventoryService.Contracts.Models;
using NUnit.Framework;
using OrderService.Contracts;
using ShoppingCartService.Controllers.Models;
using ShoppingCartServiceAcceptanceTests.Drivers;
using ShoppingCartServiceAcceptanceTests.Hooks;
using ShoppingCartServiceAcceptanceTests.RowData;

namespace ShoppingCartServiceAcceptanceTests.Steps;

[Binding]
public class ShoppingCartStepDefinitions
{
    private readonly TestServerDriver _testServerDriver;
    private readonly InventoryServiceDriver _inventoryServiceDriver;

    public ShoppingCartStepDefinitions(TestServerDriver testServerDriver, InventoryServiceDriver inventoryServiceDriver)
    {
        _testServerDriver = testServerDriver;
        _inventoryServiceDriver = inventoryServiceDriver;
    }

    private ShippingAddressDto DefaultShippingAddress =>
        new("Customer name", "USA", "NY", "12345 St.");

    private string? _lastCreatedShoppingCartId;
    private ShoppingCartDto? _lastQueriedShoppingCart;
    private HttpResponseMessage? _lastAddResponse;

    [Given(@"a user creates a new shopping cart")]
    public async Task GivenAUserCreatesANewShoppingCart()
    {
        var createShoppingCartDto = new CreateShoppingCartDto(DefaultShippingAddress);
        _lastCreatedShoppingCartId = await _testServerDriver.CreateShoppingCart(createShoppingCartDto);
    }

    [Given(@"the following items exist in inventory:")]
    public void GivenTheFollowingItemsExistInInventory(IEnumerable<ShoppingItemData> items)
    {
        var products = items.Select(i => new ProductDto(i.id, i.Name, i.Price, i.Quantity));
        _inventoryServiceDriver.AddProducts(products);
    }

    [Given(@"the product ""(.*)"" is not in the inventory")]
    public void GivenTheProductIsNotInTheInventory(string productId)
    {
        _inventoryServiceDriver.SetProductAsMissing(productId);
    }

    [When(@"a user queries for that shopping cart")]
    public async Task WhenAUserQueriesForThatShoppingCart()
    {
        _lastQueriedShoppingCart = await _testServerDriver.FindById(_lastCreatedShoppingCartId);
    }

    [Then(@"the shopping cart is found in repository")]
    public void ThenTheShoppingCartIsFoundInRepository()
    {
        var expected = new ShoppingCartDto
        {
            Id = _lastCreatedShoppingCartId!,
            ShippingAddress = DefaultShippingAddress
        };

        Assert.That(_lastQueriedShoppingCart, Is.EqualTo(expected));
    }


    [Given(@"a user adds item ""(.*)"" to shopping cart")]
    [When(@"a user adds item ""(.*)"" to shopping cart")]
    public async Task WhenAUserAddsItemToShoppingCart(string productId)
    {
        _lastAddResponse = await _testServerDriver.AddItemToShoppingCart(_lastCreatedShoppingCartId, productId);
    }

    [When(@"a user creates an order using checkout")]
    public async Task WhenAUserCreatesAnOrderUsingCheckout()
    {
        await _testServerDriver.Checkout(_lastCreatedShoppingCartId);
    }

    [Then(@"the following items exists in that shopping cart: ""(.*)""")]
    public async Task ThenTheFollowingItemsExistsInThatShoppingCart(IEnumerable<string> productIds)
    {
        var shoppingCartDto = await _testServerDriver.FindById(_lastCreatedShoppingCartId);
        Assert.That(shoppingCartDto.Items, Is.SupersetOf(productIds));
    }

    [StepArgumentTransformation]
    public IEnumerable<string> TransformStringToList(string input)
    {
        return input.Split(',');
    }

    [Then(@"then return not found")]
    public void ThenThenReturnNotFound()
    {
        Assert.That(_lastAddResponse, Is.Not.Null);
        Assert.That(_lastAddResponse!.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Then(@"a new message is queued with item ""(.*)""")]
    public async Task ThenANewMessageIsQueuedWithItem(string itemId)
    {
        var sqsClient = new AmazonSQSClient();
        var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync(SqsHooks.OrderProcessingQueueName);

        var result = await sqsClient.GetNextMessage<CreateOrderMessage>(getQueueUrlResponse.QueueUrl);

        Assert.That(result.Items, Is.EquivalentTo(new[] { $"{itemId}" }));
    }
}