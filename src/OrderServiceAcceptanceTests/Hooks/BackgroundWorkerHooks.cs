using Common.InventoryServiceFakeServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OrderService.Extensions;

namespace OrderServiceAcceptanceTests.Hooks;

/// <summary>
/// Created hook to make sure that the background service is always initialized before tests
/// </summary>
[Binding]
public class BackgroundWorkerHooks
{
    private const string InventoryServiceBaseUrlConfigKey = "ExternalServicesSettings:InventoryServiceBaseUrl";
    private const string OrderProcessingQueueNameConfigKey = "ExternalServicesSettings:OrderProcessingQueueName";
    private const string OrderBucketNameConfigKey = "ExternalServicesSettings:OrderBucketName";

    private static IHost? _host;

    [BeforeTestRun(Order = 1000)]
    public static void StartBackgroundWorker(InventoryServiceDriver inventoryServiceDriver)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureWorkerServices()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>()
                {
                    new(InventoryServiceBaseUrlConfigKey, inventoryServiceDriver.MockProviderServiceBaseUri),
                    new(OrderProcessingQueueNameConfigKey, SqsHooks.OrderProcessingQueueName),
                    new(OrderBucketNameConfigKey, S3Hooks.S3OrderBucketName),
                });
            })
            .Build();
        _host.RunAsync();
    }

    [AfterTestRun(Order = 0)]
    public static void StopBackgroundWorker()
    {
        _host?.Dispose();
        _host = null;
    }
}