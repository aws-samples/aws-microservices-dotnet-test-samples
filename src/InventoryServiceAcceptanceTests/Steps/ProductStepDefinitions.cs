using Common.TestUtils.Extensions;
using InventoryService.Contracts.Models;
using InventoryServiceAcceptanceTests.Drivers;
using InventoryServiceAcceptanceTests.RowEntries;

namespace InventoryServiceAcceptanceTests.Steps;

[Binding]
public sealed class ProductStepDefinitions
{
    private readonly TestServerDriver _testServerDriver;
    private string? _lastAddedProductId;

    public ProductStepDefinitions(TestServerDriver testServerDriver)
    {
        _testServerDriver = testServerDriver;
    }

    [Given(@"a product ""(.*)"" with price (.*) and quantity (.*) is in the inventory")]
    [When(@"a user add a new product ""(.*)"" with price (.*) and quantity (.*)")]
    public async Task WhenAUserAddANewProductWithQuantity(string name, double price, uint quantity)
    {
        var createProductDto = new CreateProductDto(name, price, quantity);

        _lastAddedProductId = await _testServerDriver.AddProductToInventory(createProductDto);
    }

    [When(@"user update product quantity to (.*)")]
    public async Task WhenUserUpdateProductQuantityTo(int quantity)
    {
        await _testServerDriver.UpdateQuantity(_lastAddedProductId!, quantity);
    }

    [Then(@"that product is found in repository with name ""(.*)"" price (.*) and quantity (.*)")]
    public async Task ThenThatProductIsFoundInRepository(string expectedName, double expectedPrice,
        uint expectedQuantity)
    {
        var actual = await _testServerDriver.FindById(_lastAddedProductId!);

        var expected = new
        {
            Name = expectedName,
            Price = expectedPrice,
            Quantity = expectedQuantity
        };

        actual.AssertEqualTo(expected);
    }

    [Then(@"the following products are in the inventory")]
    public async Task ThenTheFollowingProductsAreInTheInventory(IEnumerable<ProductData> productsData)
    {
        var expected = productsData.Select(d => new
        {
            d.Name,
            d.Price,
            d.Quantity
        });

        var actual = await _testServerDriver.GetAllProducts();

        actual.AssertEqualTo(expected);
    }
}