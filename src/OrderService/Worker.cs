using OrderService.BusinessLogic;

namespace OrderService;

public class Worker : BackgroundService
{
    private readonly OrderProcessingManager _orderProcessingManager;
    private readonly ILogger<Worker> _logger;

    public Worker(OrderProcessingManager orderProcessingManager, ILogger<Worker> logger)
    {
        _orderProcessingManager = orderProcessingManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _orderProcessingManager.ProcessNextMessage();
            await Task.Delay(10, stoppingToken);
        }
    }
}