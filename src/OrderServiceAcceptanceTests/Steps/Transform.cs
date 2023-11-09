using OrderService.BusinessLogic.Models;
using OrderServiceAcceptanceTests.RowData;
using TechTalk.SpecFlow.Assist;

namespace OrderServiceAcceptanceTests.Steps;

[Binding]
public class Transform
{
    [StepArgumentTransformation]
    public IEnumerable<ProductData> ProductDataTransformation(Table productDataTable)
    {
        return productDataTable.CreateSet<ProductData>();
    } 
    
    [StepArgumentTransformation]
    public IEnumerable<OrderItemData> OrderItemDataTransformation(Table orderItemTable)
    {
        return orderItemTable.CreateSet<OrderItemData>();
    } 
    
    [StepArgumentTransformation]
    public IEnumerable<OrderItem> OrderItemTransformation(Table orderItemTable)
    {
        return orderItemTable.CreateSet<OrderItem>();
    }

    // [StepArgumentTransformation]
    // ItemStatus ItemStatusTransform(string itemStatus)
    // {
    //     return Enum.Parse<ItemStatus>(itemStatus);
    // }
}