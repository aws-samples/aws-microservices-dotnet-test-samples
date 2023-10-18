namespace InventoryService.Contracts.Models;

public record ProductDto(string Id, string Name, double Price, uint Quantity = 0);