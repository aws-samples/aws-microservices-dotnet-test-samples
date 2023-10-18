using System.Text;
using System.Text.Json;
using Common.TestUtils.DataAccess;
using Common.TestUtils.Drivers;
using InventoryService.Contracts.Models;
using InventoryServiceAcceptanceTests.Hooks;

namespace InventoryServiceAcceptanceTests.Drivers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestServerDriver : TestServerDriverBase<Program>
    {
        private const string InventoryBaseUri = "/api/Inventory";

        public TestServerDriver() : base(
            ("InventoryDatabaseSettings:ConnectionString", MongoDbRunner.ConnectionString),
            ("InventoryDatabaseSettings:DatabaseName", MongoDbHooks.DatabaseName)
        )
        {
        }

        public async Task<string> AddProductToInventory(CreateProductDto createProductDto)
        {
            var serializeObject = JsonSerializer.Serialize(createProductDto);
            var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(InventoryBaseUri, stringContent);

            var productDto = await GetResultFromResponse<ProductDto>(response);

            return productDto.Id;
        }

        public async Task<ProductDto> FindById(string id)
        {
            var response = await Client.GetAsync($"{InventoryBaseUri}/{id}");

            return await GetResultFromResponse<ProductDto>(response);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var response = await Client.GetAsync(InventoryBaseUri);

            return await GetResultFromResponse<List<ProductDto>>(response);
        }

        public async Task DeleteProduct(string id)
        {
            var response = await Client.DeleteAsync($"{InventoryBaseUri}/{id}");

            VerifyResponse(response);
        }

        public async Task UpdateQuantity(string id, int quantity)
        {
            var serializeObject = JsonSerializer.Serialize(quantity);
            var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var response = await Client.PutAsync($"{InventoryBaseUri}/{id}/quantity", stringContent);

            VerifyResponse(response);
        }
    }
}