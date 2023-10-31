using Common.TestUtils.TestBaseClasses;
using OrderService.Contracts;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Config;
using ShoppingCartService.DataAccess;

namespace ShoppingCartServiceTests.DataAccess;

[TestFixture]
public class OrderServiceNotificationIntegrationTests : SqsTestBase
{
    [Test]
    public async Task SendOrder_SendMessageToQueue()
    {
        var target = new OrderServiceNotifications(SqsClient, new ExternalServicesSettings
        {
            OrderProcessingQueueName = QueueName
        });

        var items = new[] { "item-1", "item-2", "item-3" };
        var shippingAddress = new ShippingAddress
        {
            Name = "John Smith",
            Country = "US",
            City = "NY",
            Street = "12345 St."
        };

        await target.SendOrder(items, shippingAddress);

        var actualMessage = await GetNextMessage<CreateOrderMessage>();

        var expected = new CreateOrderMessage
        {
            CustomerName = "John Smith",
            ShippingAddress = "12345 St. NY, US",
            Items = new[] { "item-1", "item-2", "item-3" }
        };

        Assert.That(actualMessage, Is.EqualTo(expected));
    }

    [Test]
    public void SendOrder_QueueNameIsNull_ThrowApplicationException()
    {
        var target = new OrderServiceNotifications(SqsClient, new ExternalServicesSettings
        {
            OrderProcessingQueueName = null
        });

        var items = new[] { "item-1", "item-2", "item-3" };
        var shippingAddress = new ShippingAddress
        {
            Name = "John Smith",
            Country = "US",
            City = "NY",
            Street = "12345 St."
        };

        Assert.ThrowsAsync<ApplicationException>(async () => await target.SendOrder(items, shippingAddress));
    }

    [Test]
    public void SendOrder_QueueDoesNoExist_ThrowApplicationException()
    {
        var target = new OrderServiceNotifications(SqsClient, new ExternalServicesSettings
        {
            OrderProcessingQueueName = "unknown-queue"
        });

        var items = new[] { "item-1", "item-2", "item-3" };
        var shippingAddress = new ShippingAddress
        {
            Name = "John Smith",
            Country = "US",
            City = "NY",
            Street = "12345 St."
        };

        Assert.ThrowsAsync<ApplicationException>(async () => await target.SendOrder(items, shippingAddress));
    }
}