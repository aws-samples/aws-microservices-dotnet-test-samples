using System.Runtime.Serialization;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Sqs;
using OrderService.Config;
using OrderService.Contracts;

namespace OrderService.DataAccess;

public interface IIncomingOrderRepository
{
    Task<CreateOrderMessage?> GetNextOrderAsync();
}

internal class IncomingOrderRepository : IIncomingOrderRepository
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<IncomingOrderRepository> _logger;
    private readonly string? _queueName;

    public IncomingOrderRepository(IAmazonSQS sqsClient, IExternalServicesSettings servicesSettings, ILogger<IncomingOrderRepository> logger)
    {
        _sqsClient = sqsClient;
        _logger = logger;
        _queueName = servicesSettings.OrderProcessingQueueName;
    }

    public async Task<CreateOrderMessage?> GetNextOrderAsync()
    {
        var queueUrl = await _sqsClient.GetQueueUrlExAsync(_queueName);
        var request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 20
        };
        
        var response = await _sqsClient.ReceiveMessageAsync(request);

        if (response.Messages.Count == 0)
        {
            return null;
        }
        
        var message = response.Messages.Single();

        try
        {
            var result = JsonSerializer.Deserialize<CreateOrderMessage>(message.Body);

            await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);

            return result;
        }
        catch (JsonException exception)
        {
            _logger.LogCritical(exception, "Cannot handle message: {MessageBody}", message.Body);
            
            return null;
        }
    }
} 