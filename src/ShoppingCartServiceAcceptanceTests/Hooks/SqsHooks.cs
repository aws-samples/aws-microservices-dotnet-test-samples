using Common.TestUtils.DataAccess;

namespace ShoppingCartServiceAcceptanceTests.Hooks;

[Binding]
public class SqsHooks
{
    public static readonly string OrderProcessingQueueName = $"OrderProcessingQueue-{Guid.NewGuid().ToString()}";

    private static SqsTestRunner? _sqsTestRunner;

    [BeforeTestRun]
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