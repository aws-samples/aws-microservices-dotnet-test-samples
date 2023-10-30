namespace OrderService.DataAccess.Entities;

public record GetFromInventoryResult(bool HasEnoughQuantity, string Message);