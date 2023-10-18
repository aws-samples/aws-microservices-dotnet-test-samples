namespace InventoryService.Config;

public class InventoryDatabaseSettings : IInventoryDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}