using Amazon.SQS;
using Amazon.SQS.Model;

namespace Common.Sqs;

public static class AmazonSqsExtensions
{
    public static async Task<string> GetQueueUrlExAsync(this IAmazonSQS sqsClient, string? queueName)
    {
        if (queueName is null or "") throw new ApplicationException("Cannot send order, missing queue name");

        try
        {
            var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync(queueName);

            return getQueueUrlResponse.QueueUrl;
        }
        catch (QueueDoesNotExistException ex)
        {
            throw new ApplicationException($"Cannot send order, queue {queueName} does not exist", ex);
        }
    }
}