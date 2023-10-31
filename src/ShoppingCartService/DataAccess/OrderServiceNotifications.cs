using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using AutoMapper;
using Common.Sqs;
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

       try
       {
           var queueUrl = await _sqsClient.GetQueueUrlExAsync(_queueName);

            var jsonString = JsonSerializer.Serialize(message);
            var sendMessageRequest = new SendMessageRequest(queueUrl, jsonString);
            
            await _sqsClient.SendMessageAsync(sendMessageRequest);
        }
        catch (QueueDoesNotExistException ex)
        {
            throw new ApplicationException($"Cannot send order, queue {_queueName} does not exist", ex);
        }
    }
}