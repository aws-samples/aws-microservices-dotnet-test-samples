using System.Diagnostics;
using System.Net;
using ShoppingCartService.Config;

namespace ShoppingCartService.DataAccess;

class InventoryRepository : IInventoryRepository
{
    private readonly HttpClient _httpClient;

    public InventoryRepository(IExternalServicesSettings externalServicesSettings)
    {
        Debug.Assert(externalServicesSettings.InventoryServiceBaseUrl != null, "externalServicesSettings.InventoryServiceBaseUrl != null");
        
        _httpClient = new HttpClient { BaseAddress = new Uri(externalServicesSettings.InventoryServiceBaseUrl) };
    }
    public async Task<bool> ProductExist(string productId)
    {
        var response = await _httpClient.GetAsync($"/api/inventory/{productId}");

        return response.StatusCode != HttpStatusCode.NotFound;
    }
}