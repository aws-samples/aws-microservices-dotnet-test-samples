using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using AutoMapper;
using OrderService.Contracts;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Config;

namespace ShoppingCartService.DataAccess;

public class OrderServiceNotifications : IOrderServiceNotifications
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string? _queueName;

    public OrderServiceNotifications(IAmazonSQS sqsClient, IExternalServicesSettings servicesSettings)
    {
        _sqsClient = sqsClient;
        _queueName = servicesSettings.OrderProcessingQueueName;
    }
    public async Task SendOrder(IEnumerable<string> itemIds, ShippingAddress shippingAddress)
    {
        var message = new CreateOrderMessage
        {
            Items = itemIds,
            CustomerName = shippingAddress.Name,
            ShippingAddress = $"{shippingAddress.Street} {shippingAddress.City}, {shippingAddress.Country}"
        };

        if (_queueName is null or "")
        {
            throw new ApplicationException("Cannot send order, missing queue name");
        }


        try
        {
            var getQueueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName);

            var jsonString = JsonSerializer.Serialize(message);
            var sendMessageRequest = new SendMessageRequest(getQueueUrlResponse.QueueUrl, jsonString);
            
            await _sqsClient.SendMessageAsync(sendMessageRequest);
        }
        catch (QueueDoesNotExistException ex)
        {
            throw new ApplicationException($"Cannot send order, queue {_queueName} does not exist", ex);
        }
    }
}