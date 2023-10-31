using System.Text.Json;
using Amazon.SQS;
using Common.TestUtils.DataAccess;
using Common.TestUtils.Extensions;
using NUnit.Framework;

namespace Common.TestUtils.TestBaseClasses;

public class SqsTestBase
{
    private const string QueueNamePrefix = "test-order-queue";
    private SqsTestRunner? _sqsTestRunner;

    protected IAmazonSQS SqsClient => _sqsTestRunner!.SqsClient;
    protected string QueueName { get; private set; } = null!;

    [OneTimeSetUp]
    public void CreateQueue()
    {
        QueueName = $"{QueueNamePrefix}-{Guid.NewGuid()}";
        _sqsTestRunner = new SqsTestRunner(QueueName);
    }

    [OneTimeTearDown]
    public void DeleteQueue()
    {
        _sqsTestRunner?.Dispose();
        _sqsTestRunner = null;
    }

    protected async Task<T> GetNextMessage<T>()
    {
        return await SqsClient.GetNextMessage<T>(_sqsTestRunner!.QueueUrl);
    }

    protected async Task SendMessageToQueue<T>(T content)
    {
        var getQueueUrlResponse = await SqsClient.GetQueueUrlAsync(QueueName);

        var messageText = JsonSerializer.Serialize(content);

        await SqsClient.SendMessageAsync(getQueueUrlResponse.QueueUrl, messageText);
    }
}