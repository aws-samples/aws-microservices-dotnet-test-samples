using Amazon.S3;
using Amazon.SQS;
using Microsoft.Extensions.Options;
using OrderService.BusinessLogic;
using OrderService.Config;
using OrderService.DataAccess;

namespace OrderService.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureWorkerServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.Configure<ExternalServicesSettings>(
                hostContext.Configuration.GetSection(nameof(ExternalServicesSettings)));

            services.AddSingleton<IExternalServicesSettings>(sp =>
                sp.GetRequiredService<IOptions<ExternalServicesSettings>>().Value);

            services.AddHostedService<Worker>();
            services.AddAWSService<IAmazonSQS>();
            services.AddAWSService<IAmazonS3>();

            services.AddSingleton<OrderProcessingManager>();
            services.AddSingleton<IInventoryRepository, InventoryRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IIncomingOrderRepository, IncomingOrderRepository>();
        });
    }
}