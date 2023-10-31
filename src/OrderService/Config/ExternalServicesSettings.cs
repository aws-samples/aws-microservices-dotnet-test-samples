namespace OrderService.Config
{
    public class ExternalServicesSettings : IExternalServicesSettings
    {
        public string? InventoryServiceBaseUrl { get; set; }
        
        public string? OrderProcessingQueueName { get; set; }
    }
}