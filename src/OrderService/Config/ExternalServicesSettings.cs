namespace OrderService.Config;

public class ExternalServicesSettings : IExternalServicesSettings
{
    public string? InventoryServiceBaseUrl { get; init; }
    public string? OrderProcessingQueueName { get; init; }
    public string? OrderBucketName { get; init; }
}