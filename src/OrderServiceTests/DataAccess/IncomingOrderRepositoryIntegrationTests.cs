using Common.TestUtils.TestBaseClasses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OrderService.Config;
using OrderService.Contracts;
using OrderService.DataAccess;

namespace OrderServiceTests.DataAccess;

[TestFixture]
public class IncomingOrderRepositoryIntegrationTests : SqsTestBase
{
    [Test]
    public async Task GetNextMessage_MessageInQueue_ReturnMessage()
    {
        var logger = new Logger<IncomingOrderRepository>(new NullLoggerFactory());
        var settings = new ExternalServicesSettings{OrderProcessingQueueName = QueueName};
        
        var target = new IncomingOrderRepository(SqsClient, settings, logger);

        var message = new CreateOrderMessage
        {
            CustomerName = "customer-1",
            ShippingAddress = "address-1",
            Items = new[] { "item-1, item-2" }
        };

        await SendMessageToQueue(message);

        var result = await target.GetNextOrderAsync();
        
       Assert.That(result, Is.EqualTo(message));
    }    
    
    [Test]
    public async Task GetNextMessage_NoMessageInQueue_ReturnNull()
    {
        var logger = new Logger<IncomingOrderRepository>(new NullLoggerFactory());
        var settings = new ExternalServicesSettings{OrderProcessingQueueName = QueueName};
        
        var target = new IncomingOrderRepository(SqsClient, settings, logger);

        var result = await target.GetNextOrderAsync(1);
        
       Assert.That(result, Is.Null);
    }
}