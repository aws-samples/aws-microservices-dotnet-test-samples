using System.Text.Json;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using NUnit.Framework;
using OrderService.BusinessLogic.Models;
using OrderService.Contracts;
using OrderServiceAcceptanceTests.Hooks;
using OrderServiceAcceptanceTests.RowData;

namespace OrderServiceAcceptanceTests.Steps;

[Binding]
public class OrderStepDefinitions
{
    private CreateOrderMessage? _lastQueuedMessage;

    [When(@"a new message with arrives with the following products:")]
    public async Task WhenANewMessageWithArrivesWithTheFollowingProducts(IEnumerable<OrderItemData> orderItemData)
    {
        _lastQueuedMessage = new CreateOrderMessage(Guid.NewGuid().ToString())
        {
            Items = orderItemData.Select(o => o.Id),
            CustomerName = "customer-1",
            ShippingAddress = "12345 St. New York, New York"
        };

        var sqsClient = new AmazonSQSClient();
        var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync(SqsHooks.OrderProcessingQueueName);

        await sqsClient.SendMessageAsync(new SendMessageRequest
        {
            MessageBody = JsonSerializer.Serialize(_lastQueuedMessage),
            QueueUrl = getQueueUrlResponse.QueueUrl
        });
    }

    [Then(@"that order is saved as ""(.*)"" with items")]
    public async Task ThenThatOrderIsSavedAsWithAllItemsMarkedAsReady(string status, IEnumerable<OrderItem> orderItems)
    {
        Assert.That(_lastQueuedMessage, Is.Not.Null);

        using var s3Client = new AmazonS3Client();

        var count = 0;
        bool itemFound;
        var expectedKey = $"{status}/{_lastQueuedMessage?.Id}";
        var bucketName = S3Hooks.S3OrderBucketName;
        do
        {
            await Task.Delay(100);
            var listObjectsResponse = await s3Client.ListObjectsAsync(bucketName);
            itemFound = listObjectsResponse.S3Objects.Any(o => o.Key == expectedKey);
        } while (itemFound is false && count++ <= 60);

        Assert.That(itemFound, Is.EqualTo(true), $"Did not file S3 file {expectedKey}");

        var result = await s3Client.GetObjectAsync(bucketName, expectedKey);

        var actual = JsonSerializer.Deserialize<Order>(result.ResponseStream);

        var expected = new Order(_lastQueuedMessage!.Id,
            _lastQueuedMessage!.CustomerName,
            _lastQueuedMessage!.ShippingAddress,
            orderItems);

        Assert.That(actual, Is.EqualTo(expected));

    }
}