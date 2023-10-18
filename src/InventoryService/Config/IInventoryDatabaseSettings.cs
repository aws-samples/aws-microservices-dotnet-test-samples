namespace InventoryService.Config;

public interface IInventoryDatabaseSettings
{
    string ConnectionString { get; }
    string DatabaseName { get; }
}