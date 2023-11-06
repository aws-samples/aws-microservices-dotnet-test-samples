using OrderService.BusinessLogic.Models;

namespace OrderServiceTests.BusinessLogic.Models;

[TestFixture]
public class OrderUnitTests
{
    [Test]
    public void OrderHasNoItems_StatusIsNoItemsInOrder()
    {
        var target = new Order("", "", "", Array.Empty<OrderItem>());

        Assert.That(target.Status, Is.EqualTo(OrderStatus.NoItemsInOrder));
    }

    [TestCase(ItemStatus.NotInInventory, OrderStatus.MissingItems)]
    [TestCase(ItemStatus.Ready, OrderStatus.ReadyForShipping)]
    public void OrderHasOneItem_StatusBasedOnItem(ItemStatus itemStatus, OrderStatus expected)
    {
        var target = new Order("", "", "", new[]
        {
            new OrderItem("", itemStatus)
        });

        Assert.That(target.Status, Is.EqualTo(expected));
    }

    [Test]
    public void OrderHasMultipleItemsOneIsNotReady_OrderStatusIsMissingItems()
    {
        var target = new Order("", "", "", new[]
        {
            new OrderItem("", ItemStatus.Ready),
            new OrderItem("", ItemStatus.NotInInventory)
        });

        Assert.That(target.Status, Is.EqualTo(OrderStatus.MissingItems));
    }
}