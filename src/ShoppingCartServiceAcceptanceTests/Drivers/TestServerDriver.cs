using System.Net;
using System.Text;
using System.Text.Json;
using Common.TestUtils.DataAccess;
using Common.TestUtils.Drivers;
using NUnit.Framework;
using ShoppingCartService.Controllers.Models;
using ShoppingCartServiceAcceptanceTests.Hooks;

namespace ShoppingCartServiceAcceptanceTests.Drivers;

public class TestServerDriver : TestServerDriverBase<Program>
{
    private const string ShoppingCartBaseUri = "/api/ShoppingCart";

    private const string DynamodbLocalModeConfigKey = "DynamoDb:LocalMode";
    private const string DynamodbLocalServiceUrlConfigKey = "DynamoDb:LocalServiceUrl";
    private const string InventoryServiceBaseUrlConfigKey = "ExternalServicesSettings:InventoryServiceBaseUrl";
    private const string OrderProcessingQueueNameConfigKey = "ExternalServicesSettings:OrderProcessingQueueName";

    public TestServerDriver(InventoryServiceDriver inventoryServiceDriver, DynamoDbRunner dynamoDbRunner)
        : base(
            (DynamodbLocalModeConfigKey, "true"),
            (DynamodbLocalServiceUrlConfigKey, $"http://localhost:{dynamoDbRunner.ExternalPort}"),
            (InventoryServiceBaseUrlConfigKey, inventoryServiceDriver.MockProviderServiceBaseUri),
            (OrderProcessingQueueNameConfigKey, SqsHooks.OrderProcessingQueueName))
    {
    }

    public async Task<string> CreateShoppingCart(CreateShoppingCartDto createShoppingCartDto)
    {
        var serializeObject = JsonSerializer.Serialize(createShoppingCartDto);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

        var response = await Client.PostAsync(ShoppingCartBaseUri, stringContent);

        var shoppingCartDto = await GetResultFromResponse<ShoppingCartDto>(response);

        return shoppingCartDto.Id;
    }

    public async Task<ShoppingCartDto> FindById(string? shoppingCartId)
    {
        Assert.That(shoppingCartId, Is.Not.Null);

        var response = await Client.GetAsync($"{ShoppingCartBaseUri}/{shoppingCartId}");

        return await GetResultFromResponse<ShoppingCartDto>(response);
    }

    public async Task<HttpResponseMessage> AddItemToShoppingCart(string? shoppingCartId, string productId)
    {
        Assert.That(shoppingCartId, Is.Not.Null);

        return await Client.PutAsync($"{ShoppingCartBaseUri}/{shoppingCartId}/item/{productId}", null);
    }

    public async Task Checkout(string? shoppingCartId)
    {
        Assert.That(shoppingCartId, Is.Not.Null);

        var result = await Client.PostAsync($"/checkout/{shoppingCartId}", null);

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}