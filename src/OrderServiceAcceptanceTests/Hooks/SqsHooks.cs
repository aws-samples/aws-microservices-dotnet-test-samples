using Common.TestUtils.DataAccess;

namespace OrderServiceAcceptanceTests.Hooks;

[Binding]
public class SqsHooks
{
    public static readonly string OrderProcessingQueueName = $"OrderProcessingQueue-{Guid.NewGuid().ToString()}";

    private static SqsTestRunner? _sqsTestRunner;

    [BeforeTestRun(Order = 0)]
    public static void BeforeTestRun()
    {
        _sqsTestRunner = new SqsTestRunner(OrderProcessingQueueName);
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        _sqsTestRunner?.Dispose();
        _sqsTestRunner = null;
    }
}