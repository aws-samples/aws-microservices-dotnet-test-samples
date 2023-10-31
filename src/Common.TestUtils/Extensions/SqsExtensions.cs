using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using NUnit.Framework;

namespace Common.TestUtils.Extensions;

public static class SqsExtensions
{
    public static async Task<T> GetNextMessage<T>(this IAmazonSQS sqsClient, string queueUrl)
    {
        var message = await GetNextSqsMessage(sqsClient, queueUrl);

        return JsonSerializer.Deserialize<T>(message.Body)!;
    }

    public static async Task<Message> GetNextSqsMessage(this IAmazonSQS sqsClient, string queueUrl)
    {
        var receiveMessageResponse = await GetNextMessageInternal(sqsClient, queueUrl);

        Assert.NotNull(receiveMessageResponse);
        Assert.That(receiveMessageResponse.Messages, Is.Not.Empty);

        var message = receiveMessageResponse.Messages[0];
        await sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);

        Assert.NotNull(message.Body);
        return message;
    }

    private static async Task<ReceiveMessageResponse> GetNextMessageInternal(IAmazonSQS sqsClient, string queueUrl)
    {
        return await GetNextMessageInternal(sqsClient, queueUrl, _ => true);
    }

    private static async Task<ReceiveMessageResponse> GetNextMessageInternal(IAmazonSQS sqsClient, string queueUrl,
        Predicate<Message> checkForMessage)
    {
        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,

            // Handle massages one by one
            MaxNumberOfMessages = 1,

            // make sure messages are handled even if run by concurrent test
            VisibilityTimeout = 1,

            // use long polling to improve response time
            WaitTimeSeconds = 20
        };

        var count = 0;
        ReceiveMessageResponse? receiveMessageResponse;
        do
        {
            receiveMessageResponse = await sqsClient.ReceiveMessageAsync(receiveMessageRequest);
            if (receiveMessageResponse.Messages.Count != 0 &&
                checkForMessage.Invoke(receiveMessageResponse.Messages[0]))
                break;
        } while (count++ < 30); // 20 secs * 30 = 10 min max

        return receiveMessageResponse;
    }
}