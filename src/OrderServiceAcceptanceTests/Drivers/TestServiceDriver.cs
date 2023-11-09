// // using System;
// // using Common.InventoryServiceFakeServer;
// // using Common.TestUtils.Drivers;
// // using OrderServiceAcceptanceTests.Hooks;
// //
// // namespace OrderServiceAcceptanceTests.Drivers
// // {
// //     public class TestServiceDriver : TestServerDriverBase<Program>
// //     {
// //         private const string InventoryServiceBaseUrlConfigKey = "ExternalServicesSettings:InventoryServiceBaseUrl";
// //         private const string OrderProcessingQueueNameConfigKey = "ExternalServicesSettings:OrderProcessingQueueName";
// //         private const string OrderBucketNameConfigKey = "ExternalServicesSettings:OrderBucketName";
// //
// //         
// //         public TestServiceDriver(InventoryServiceDriver inventoryServiceDriver) : base(
// //             (InventoryServiceBaseUrlConfigKey, inventoryServiceDriver.MockProviderServiceBaseUri),
// //             (OrderProcessingQueueNameConfigKey, SqsHooks.OrderProcessingQueueName),
// //             (OrderBucketNameConfigKey, S3Hooks.S3OrderBucketName)
// //             )
// //         {
// //             
// //         }
// //     }
// // }
//
// using Amazon.S3;
// using Amazon.SQS;
// using Common.InventoryServiceFakeServer;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Options;
// using OrderService;
// using OrderService.BusinessLogic;
// using OrderService.Config;
// using OrderService.DataAccess;
// using OrderServiceAcceptanceTests.Hooks;
//
// namespace OrderServiceAcceptanceTests.Drivers
// {
//     public class BackgroundTestService : IDisposable
//     {
//         private const string InventoryServiceBaseUrlConfigKey = "ExternalServicesSettings:InventoryServiceBaseUrl";
//         private const string OrderProcessingQueueNameConfigKey = "ExternalServicesSettings:OrderProcessingQueueName";
//         private const string OrderBucketNameConfigKey = "ExternalServicesSettings:OrderBucketName";
//
//         private readonly IHost _host;
//         private readonly Task _task;
//
//         public BackgroundTestService(InventoryServiceDriver inventoryServiceDriver)
//         {
//             var settings = new ExternalServicesSettings
//             {
//                 InventoryServiceBaseUrl = inventoryServiceDriver.MockProviderServiceBaseUri,
//                 OrderProcessingQueueName = SqsHooks.OrderProcessingQueueName,
//                 OrderBucketName = S3Hooks.S3OrderBucketName
//             };
//
//             _host = Host.CreateDefaultBuilder()
//                 .ConfigureServices((hostContext, services) =>
//                 {
//                     services.Configure<ExternalServicesSettings>(
//                         hostContext.Configuration.GetSection(nameof(ExternalServicesSettings)));
//
//                     services.AddSingleton<IExternalServicesSettings>(sp =>
//                         sp.GetRequiredService<IOptions<ExternalServicesSettings>>().Value);
//                     services.AddHostedService<Worker>();
//                     services.AddAWSService<IAmazonSQS>();
//                     services.AddAWSService<IAmazonS3>();
//
//                     services.AddSingleton<OrderProcessingManager>();
//                     services.AddSingleton<IInventoryRepository, InventoryRepository>();
//                     services.AddSingleton<IOrderRepository, OrderRepository>();
//                     services.AddSingleton<IIncomingOrderRepository, IncomingOrderRepository>();
//                 }).ConfigureAppConfiguration(builder =>
//                 {
//                     builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>()
//                     {
//                         new (InventoryServiceBaseUrlConfigKey, inventoryServiceDriver.MockProviderServiceBaseUri),
//                         new (OrderProcessingQueueNameConfigKey, SqsHooks.OrderProcessingQueueName),
//                         new (OrderBucketNameConfigKey, S3Hooks.S3OrderBucketName),
//                     });
//                 })
//                 .Build();
//            _task = _host.RunAsync();
//         }
//
//         public void Dispose()
//         {
//             _host.Dispose();
//         }
//     }
// }