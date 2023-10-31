using InventoryServiceAcceptanceTests.RowEntries;
using TechTalk.SpecFlow.Assist;

namespace InventoryServiceAcceptanceTests.Steps;

[Binding]
public class Transform
{
    [StepArgumentTransformation]
    public IEnumerable<ProductData> ProductDataTransformation(Table productDataTable)
    {
        return productDataTable.CreateSet<ProductData>();
    }
}