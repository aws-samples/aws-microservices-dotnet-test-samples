using OrderService.Contracts;

namespace OrderService.DataAccess;

public interface IIncomingOrderRepository
{
    Task<CreateOrderMessage> GetNextOrderAsync();
} 