using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using OrderService.BusinessLogic;
using OrderService.BusinessLogic.Models;
using OrderService.Contracts;
using OrderService.DataAccess;
using OrderService.DataAccess.Entities;

namespace OrderServiceTests.BusinessLogic;

[TestFixture]
public class OrderProcessingManagerUnitTests
{
    [Test]
    public async Task ProcessNextMessage_MessageReceivedAndHasEnoughQuantity_SaveAsOrderAsReady()
    {
        using var autoFake = new AutoFake();

        var fakeIncomingOrderRepository = autoFake.Resolve<IIncomingOrderRepository>();
        var createOrderMessage = new CreateOrderMessage("message-1")
        {
            CustomerName = "customer-1",
            ShippingAddress = "address-1",
            Items = new[] { "item-1" }
        };

        A.CallTo(() => fakeIncomingOrderRepository.GetNextOrderAsync(A<int>._))
            .Returns(Task.FromResult<CreateOrderMessage?>(createOrderMessage));

        var fakeInventoryRepository = autoFake.Resolve<IInventoryRepository>();
        A.CallTo(() => fakeInventoryRepository.GetItemFromInventory("item-1"))
            .Returns(Task.FromResult(new GetFromInventoryResult(true, string.Empty)));

        var fakeOrderRepository = autoFake.Resolve<IOrderRepository>();
        var target = autoFake.Resolve<OrderProcessingManager>();

        await target.ProcessNextMessage();

        var expected = new Order("message-1", "customer-1",
            "address-1",
            new[] { new OrderItem("item-1", ItemStatus.Ready) });

        A.CallTo(() => fakeOrderRepository.SaveOrderAsync(expected))
            .MustHaveHappened();
    }

    [Test]
    public async Task ProcessNextMessage_MessageReceivedAndDoNotEnoughQuantityForItem_SaveAsOrderWithMissingProduct()
    {
        using var autoFake = new AutoFake();

        var fakeIncomingOrderRepository = autoFake.Resolve<IIncomingOrderRepository>();
        var createOrderMessage = new CreateOrderMessage("message-1")
        {
            CustomerName = "customer-1",
            ShippingAddress = "address-1",
            Items = new[] { "item-1" }
        };

        A.CallTo(() => fakeIncomingOrderRepository.GetNextOrderAsync(A<int>._))
            .Returns(Task.FromResult<CreateOrderMessage?>(createOrderMessage));

        var fakeInventoryRepository = autoFake.Resolve<IInventoryRepository>();
        A.CallTo(() => fakeInventoryRepository.GetItemFromInventory("item-1"))
            .Returns(Task.FromResult(new GetFromInventoryResult(false, string.Empty)));

        var fakeOrderRepository = autoFake.Resolve<IOrderRepository>();
        var target = autoFake.Resolve<OrderProcessingManager>();

        await target.ProcessNextMessage();

        var expected = new Order("message-1", "customer-1",
            "address-1",
            new[] { new OrderItem("item-1", ItemStatus.NotInInventory) });

        A.CallTo(() => fakeOrderRepository.SaveOrderAsync(expected))
            .MustHaveHappened();
    }

    [Test]
    public async Task
        ProcessNextMessage_MessageReceivedAndSomeItemsDoNotEnoughQuantityForItem_SaveAsOrderWithMissingProduct()
    {
        using var autoFake = new AutoFake();

        var fakeIncomingOrderRepository = autoFake.Resolve<IIncomingOrderRepository>();
        var createOrderMessage = new CreateOrderMessage("message-1")
        {
            CustomerName = "customer-1",
            ShippingAddress = "address-1",
            Items = new[] { "item-1", "item-2" }
        };

        A.CallTo(() => fakeIncomingOrderRepository.GetNextOrderAsync(A<int>._))
            .Returns(Task.FromResult<CreateOrderMessage?>(createOrderMessage));

        var fakeInventoryRepository = autoFake.Resolve<IInventoryRepository>();
        A.CallTo(() => fakeInventoryRepository.GetItemFromInventory("item-1"))
            .Returns(Task.FromResult(new GetFromInventoryResult(true, string.Empty)));

        A.CallTo(() => fakeInventoryRepository.GetItemFromInventory("item-2"))
            .Returns(Task.FromResult(new GetFromInventoryResult(false, string.Empty)));


        var target = autoFake.Resolve<OrderProcessingManager>();

        await target.ProcessNextMessage();

        var expected = new Order("message-1", "customer-1",
            "address-1",
            new[]
            {
                new OrderItem("item-1", ItemStatus.Ready),
                new OrderItem("item-2", ItemStatus.NotInInventory)
            });

        var fakeOrderRepository = autoFake.Resolve<IOrderRepository>();
        A.CallTo(() => fakeOrderRepository.SaveOrderAsync(expected))
            .MustHaveHappened();
    }

    [Test]
    public async Task ProcessNextMessage_RepositoryReturnNull_DoNotCallOtherRepositories()
    {
        using var autoFake = new AutoFake();

        var fakeIncomingOrderRepository = autoFake.Resolve<IIncomingOrderRepository>();

        A.CallTo(() => fakeIncomingOrderRepository.GetNextOrderAsync(A<int>._))
            .Returns(Task.FromResult<CreateOrderMessage?>(null));

        var target = autoFake.Resolve<OrderProcessingManager>();

        await target.ProcessNextMessage();

        var fakeInventoryRepository = autoFake.Resolve<IInventoryRepository>();
        var fakeOrderRepository = autoFake.Resolve<IOrderRepository>();

        Assert.Multiple(() =>
            {
                A.CallTo(() => fakeInventoryRepository.GetItemFromInventory(A<string>._))
                    .MustNotHaveHappened();

                A.CallTo(() => fakeOrderRepository.SaveOrderAsync(A<Order>._))
                    .MustNotHaveHappened();
            }
        );
    }
}