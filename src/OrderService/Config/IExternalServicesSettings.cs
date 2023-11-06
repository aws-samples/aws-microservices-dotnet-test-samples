namespace OrderService.Config;

public interface IExternalServicesSettings
{
    string? InventoryServiceBaseUrl { get; }

    string? OrderProcessingQueueName { get; }
    
    string? OrderBucketName { get; }
}