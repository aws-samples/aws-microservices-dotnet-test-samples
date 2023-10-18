using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using NUnit.Framework;

namespace Common.TestUtils.DataAccess;

public class SqsTestRunner : IDisposable
{
    public IAmazonSQS SqsClient { get; } = new AmazonSQSClient();
    private string QueueName { get; }
    private const string QueueNamePrefix = "test-order-queue";
    public string QueueUrl { get; }

    public SqsTestRunner(string queueName)
    {
        QueueName = queueName;
        var createQueueResponse = SqsClient.CreateQueueAsync(QueueName).Result;
        QueueUrl = createQueueResponse.QueueUrl;
    }
    
    public void Dispose()
    {
        SqsClient.DeleteQueueAsync(QueueUrl).Wait();
        SqsClient.Dispose();
    }
}