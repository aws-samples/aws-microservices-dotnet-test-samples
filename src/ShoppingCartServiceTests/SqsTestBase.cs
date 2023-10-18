using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common.TestUtils.DataAccess;
using Common.TestUtils.Extensions;

namespace ShoppingCartServiceTests;

public class SqsTestBase
{
    private const string QueueNamePrefix = "test-order-queue";
    private SqsTestRunner? _sqsTestRunner;

    protected IAmazonSQS SqsClient => _sqsTestRunner!.SqsClient;
    protected string QueueName { get; set; } = null!;
    
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
}