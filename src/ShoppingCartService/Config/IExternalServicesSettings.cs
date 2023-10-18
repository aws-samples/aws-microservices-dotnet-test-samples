namespace ShoppingCartService.Config
{
    public interface IExternalServicesSettings
    {
        string? InventoryServiceBaseUrl { get; }
        
        string? OrderProcessingQueueName { get; }
    }
}