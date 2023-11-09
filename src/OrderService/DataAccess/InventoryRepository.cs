using System.Diagnostics;
using System.Net;
using System.Text.Json;
using InventoryService.Contracts.Models;
using OrderService.Config;

namespace OrderService.DataAccess;

internal class InventoryRepository : IInventoryRepository
{
    private readonly HttpClient _httpClient;

    public InventoryRepository(IExternalServicesSettings externalServicesSettings)
    {
        Debug.Assert(externalServicesSettings.InventoryServiceBaseUrl != null,
            "externalServicesSettings.InventoryServiceBaseUrl != null");

        _httpClient = new HttpClient { BaseAddress = new Uri(externalServicesSettings.InventoryServiceBaseUrl) };
    }

    public async Task<bool> ProductExist(string productId)
    {
        var response = await _httpClient.GetAsync($"/api/inventory/{productId}");

        return response.StatusCode != HttpStatusCode.NotFound;
    }

    public async Task<bool> CheckItemQuantity(string productId, uint quantity)
    {
        var response = await _httpClient.GetAsync($"/api/inventory/{productId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        
        var responseJson = await response.Content.ReadAsStringAsync();

        var   options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var product = JsonSerializer.Deserialize<ProductDto>(responseJson, options);
        
        return product!.Quantity >= quantity;
    }
}