using Amazon.SQS;

namespace Common.TestUtils.DataAccess;

public class SqsTestRunner : IDisposable
{
    private const string QueueNamePrefix = "test-order-queue";

    public SqsTestRunner(string queueName)
    {
        QueueName = queueName;
        var createQueueResponse = SqsClient.CreateQueueAsync(QueueName).Result;
        QueueUrl = createQueueResponse.QueueUrl;
    }

    public IAmazonSQS SqsClient { get; } = new AmazonSQSClient();
    private string QueueName { get; }
    public string QueueUrl { get; }

    public void Dispose()
    {
        SqsClient.DeleteQueueAsync(QueueUrl).Wait();
        SqsClient.Dispose();
    }
}