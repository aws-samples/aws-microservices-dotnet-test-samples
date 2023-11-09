using System.Text.Json;
using Amazon.S3;
using OrderService.BusinessLogic.Models;
using OrderService.Config;

namespace OrderService.DataAccess;

internal class OrderRepository : IOrderRepository
{
    private readonly IAmazonS3 _s3Client;
    private readonly string? _bucketName;

    public OrderRepository(IAmazonS3 s3Client, IExternalServicesSettings externalServicesSettings)
    {
        _s3Client = s3Client;
        _bucketName = externalServicesSettings.OrderBucketName;
    }

    public async Task SaveOrderAsync(Order order)
    {
        var serialized = JsonSerializer.Serialize(order);
        using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream);
        await writer.WriteAsync(serialized);
        await writer.FlushAsync();
        memoryStream.Position = 0;

        await _s3Client.UploadObjectFromStreamAsync(_bucketName, 
            $"{order.Status}/{order.Id}", 
            memoryStream,
            new Dictionary<string, object>());
    }
}