using InventoryService.Contracts.Models;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ShoppingCartServiceAcceptanceTests.Drivers;

public class InventoryServiceDriver: IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();
    
    public string MockProviderServiceBaseUri => _server.Urls[0];

    public void Dispose()
    {
        _server.Dispose();
    }
    
    public void AddProducts(IEnumerable<ProductDto> products)
    {
        foreach (var product in products)
        {
            var responseString = JsonSerializer.Serialize(product);

            _server.Given(Request.Create().WithPath($"/api/inventory/{product.Id}"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithBody(responseString));
        }
    }

    public void SetProductAsMissing(string productId)
    {
        _server.Given(Request.Create().WithPath($"/api/inventory/{productId}"))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(404));
    }
}