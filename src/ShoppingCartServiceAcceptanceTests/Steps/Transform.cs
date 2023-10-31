using ShoppingCartServiceAcceptanceTests.RowData;
using TechTalk.SpecFlow.Assist;

namespace ShoppingCartServiceAcceptanceTests.Steps;

[Binding]
public class Transform
{
    [StepArgumentTransformation]
    public IEnumerable<ShoppingItemData> ProductDataTransformation(Table productDataTable)
    {
        return productDataTable.CreateSet<ShoppingItemData>();
    }
}