using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OrderService.Config;
using OrderService.Contracts;
using OrderService.DataAccess;

namespace OrderServiceTests.DataAccess;

[TestFixture]
public class IncomingOrderRepositoryUnitTests
{
    private static IncomingOrderRepository CreateIncomingOrderRepository(string orderProcessingQueueName,
        IAmazonSQS fakeSqsClient)
    {
        var settings = new ExternalServicesSettings
        {
            OrderProcessingQueueName = orderProcessingQueueName
        };

        var logger = new Logger<IncomingOrderRepository>(new NullLoggerFactory());
        var target = new IncomingOrderRepository(fakeSqsClient, settings, logger);
        return target;
    }

    [Test]
    public async Task GetNextOrderAsync_CallSqsUsingLogPolling()
    {
        const string orderProcessingQueueName = "test-queue";
        const string queueUrl = "http://test-queue-url";

        var fakeSqsClient = A.Fake<IAmazonSQS>();

        A.CallTo(() => fakeSqsClient.GetQueueUrlAsync(orderProcessingQueueName, A<CancellationToken>._))
            .Returns(Task.FromResult(new GetQueueUrlResponse
            {
                QueueUrl = queueUrl
            }));

        var target = CreateIncomingOrderRepository(orderProcessingQueueName, fakeSqsClient);

        ReceiveMessageRequest? actual = null;

        A.CallTo(() =>
                fakeSqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
            .Invokes(call => actual = call.Arguments.Get<ReceiveMessageRequest>(0));

        await target.GetNextOrderAsync();

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.WaitTimeSeconds, Is.GreaterThan(0));
    }


    [Test]
    public async Task GetNextOrder_ValidOrder_CallDeleteMessage()
    {
        const string orderProcessingQueueName = "test-queue";
        const string queueUrl = "http://test-queue-url";

        var fakeSqsClient = A.Fake<IAmazonSQS>();

        A.CallTo(() => fakeSqsClient.GetQueueUrlAsync(orderProcessingQueueName, A<CancellationToken>._))
            .Returns(Task.FromResult(new GetQueueUrlResponse
            {
                QueueUrl = queueUrl
            }));

        var message = JsonSerializer.Serialize(new CreateOrderMessage("message-1"));
        A.CallTo(() => fakeSqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(new ReceiveMessageResponse
            {
                Messages = new List<Message>
                {
                    new()
                    {
                        MessageId = "message-1",
                        ReceiptHandle = "handle-1",
                        Body = message
                    }
                }
            }));

        var target = CreateIncomingOrderRepository(orderProcessingQueueName, fakeSqsClient);

        await target.GetNextOrderAsync();

        A.CallTo(() => fakeSqsClient.DeleteMessageAsync(queueUrl, "handle-1", A<CancellationToken>._))
            .MustHaveHappened();
    }

    [Test]
    public async Task GetNextOrder_ValidOrder_ReturnMessage()
    {
        const string orderProcessingQueueName = "test-queue";
        const string queueUrl = "http://test-queue-url";

        var fakeSqsClient = A.Fake<IAmazonSQS>();

        A.CallTo(() => fakeSqsClient.GetQueueUrlAsync(orderProcessingQueueName, A<CancellationToken>._))
            .Returns(Task.FromResult(new GetQueueUrlResponse
            {
                QueueUrl = queueUrl
            }));

        var createOrderMessage = new CreateOrderMessage("message-1")
        {
            CustomerName = "customer-1",
            ShippingAddress = "shipping-1",
            Items = new[] { "item-1", "item-2" }
        };
        var message = JsonSerializer.Serialize(createOrderMessage);
        A.CallTo(() => fakeSqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(new ReceiveMessageResponse
            {
                Messages = new List<Message>
                {
                    new()
                    {
                        MessageId = "message-1",
                        ReceiptHandle = "handle-1",
                        Body = message
                    }
                }
            }));

        var target = CreateIncomingOrderRepository(orderProcessingQueueName, fakeSqsClient);


        var result = await target.GetNextOrderAsync();

        Assert.That(result, Is.EqualTo(createOrderMessage));
    }

    [Test]
    public async Task GetNextOrder_NoMessageReturned_ReturnNull()
    {
        const string orderProcessingQueueName = "test-queue";
        const string queueUrl = "http://test-queue-url";

        var fakeSqsClient = A.Fake<IAmazonSQS>();

        A.CallTo(() => fakeSqsClient.GetQueueUrlAsync(orderProcessingQueueName, A<CancellationToken>._))
            .Returns(Task.FromResult(new GetQueueUrlResponse
            {
                QueueUrl = queueUrl
            }));

        A.CallTo(() => fakeSqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(new ReceiveMessageResponse
            {
                Messages = new List<Message>()
            }));

        var target = CreateIncomingOrderRepository(orderProcessingQueueName, fakeSqsClient);


        var result = await target.GetNextOrderAsync();

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetNextOrder_ErrorParsingMessage_DoNotCallDelete()
    {
        const string orderProcessingQueueName = "test-queue";
        const string queueUrl = "http://test-queue-url";

        var fakeSqsClient = A.Fake<IAmazonSQS>();

        A.CallTo(() => fakeSqsClient.GetQueueUrlAsync(orderProcessingQueueName, A<CancellationToken>._))
            .Returns(Task.FromResult(new GetQueueUrlResponse
            {
                QueueUrl = queueUrl
            }));


        A.CallTo(() => fakeSqsClient.ReceiveMessageAsync(A<ReceiveMessageRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(new ReceiveMessageResponse
            {
                Messages = new List<Message>
                {
                    new()
                    {
                        ReceiptHandle = "handle-1",
                        Body = "dummy string"
                    }
                }
            }));

        var target = CreateIncomingOrderRepository(orderProcessingQueueName, fakeSqsClient);

        await target.GetNextOrderAsync();

        A.CallTo(() =>
                fakeSqsClient.DeleteMessageAsync(queueUrl, "handle-1", A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}