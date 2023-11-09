using Common.InventoryServiceFakeServer;
using InventoryService.Contracts.Models;
using OrderService.Contracts;
using OrderServiceAcceptanceTests.RowData;

namespace OrderServiceAcceptanceTests.Steps;

[Binding]
public sealed class InventoryStepDefinitions
{
    private readonly InventoryServiceDriver _inventoryServiceDriver;

    public InventoryStepDefinitions(InventoryServiceDriver inventoryServiceDriver)
    {
        _inventoryServiceDriver = inventoryServiceDriver;
    }

    [Given(@"the following products are in the inventory")]
    public void GivenTheFollowingProductsAreInTheInventory(IEnumerable<ProductData> items)
    {
        var products = items.Select(i => new ProductDto(i.Id, i.Name, i.Price, i.Quantity));
        
        _inventoryServiceDriver.AddProducts(products);
    }
    
       
}