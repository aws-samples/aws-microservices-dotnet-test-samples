using OrderService.BusinessLogic.Models;

namespace OrderService.DataAccess;


public interface IOrderRepository
{
    Task SaveOrderAsync(Order order);
}

