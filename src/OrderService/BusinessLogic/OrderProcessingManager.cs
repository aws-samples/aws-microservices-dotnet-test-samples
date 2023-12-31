using OrderService.BusinessLogic.Models;
using OrderService.DataAccess;

namespace OrderService.BusinessLogic;

public class OrderProcessingManager
{
    private readonly IIncomingOrderRepository _incomingOrderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderProcessingManager(IIncomingOrderRepository incomingOrderRepository,
        IInventoryRepository inventoryRepository, IOrderRepository orderRepository)
    {
        _incomingOrderRepository = incomingOrderRepository;
        _inventoryRepository = inventoryRepository;
        _orderRepository = orderRepository;
    }

    public async Task ProcessNextMessage()
    {
        var createOrderMessage = await _incomingOrderRepository.GetNextOrderAsync();

        if (createOrderMessage is null) return;

        var orderItems = new List<OrderItem>();

        foreach (var productId in createOrderMessage.Items)
        {
            var hasEnoughQuantity = await _inventoryRepository.CheckItemQuantity(productId, 1);
            var itemStatus = hasEnoughQuantity ? ItemStatus.Ready : ItemStatus.NotInInventory;

            orderItems.Add(new OrderItem(productId, itemStatus));
        }

        var order = new Order(createOrderMessage.Id, createOrderMessage.CustomerName,
            createOrderMessage.ShippingAddress,
            orderItems);

        await _orderRepository.SaveOrderAsync(order);
    }
}