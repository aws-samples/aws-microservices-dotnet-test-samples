namespace OrderService.BusinessLogic.Models;

public record OrderItem(string ProductId, ItemStatus ItemStatus)
{
    public override string ToString()
    {
        return $"{nameof(ProductId)}: {ProductId}, {nameof(ItemStatus)}: {ItemStatus}";
    }
}