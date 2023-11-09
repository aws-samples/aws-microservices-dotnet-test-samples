using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Common.TestUtils.TestBaseClasses;
using OrderService.BusinessLogic.Models;
using OrderService.Config;
using OrderService.DataAccess;

namespace OrderServiceTests.DataAccess;

[TestFixture]
public class OrderRepositoryIntegrationTests : S3TestBase
{
    [TearDown]
    public async Task DeleteAllFilesInBucket()
    {
        await DeleteAllFilesAsync();
    }
    
    [Test]
    public async Task SaveOrderAsync_OrderStatusReady_SaveOrderAsJsonInReadySubFolder()
    {
        var s3Client = new AmazonS3Client();

        var settings = new ExternalServicesSettings
        {
            OrderBucketName = BucketName
        };
        var target = new OrderRepository(s3Client,settings);

        var orderName = $"order-{Guid.NewGuid()}";
        var order = new Order(orderName, "customer-1", "address-1", new[]
        {
            new OrderItem("product-1", ItemStatus.Ready)
        });

        await target.SaveOrderAsync(order);

        var expectedOrder = $"ReadyForShipping/{orderName}";
        var getObjectResponse = await s3Client.GetObjectAsync(BucketName, expectedOrder);

        await using var stream = getObjectResponse.ResponseStream;
        using var streamReader = new StreamReader(stream);

        var actualJson = await streamReader.ReadToEndAsync();
        var actual = JsonSerializer.Deserialize<Order>(actualJson);
        
        Assert.That(actual, Is.EqualTo(order));
    }
    
    [Test]
    public async Task SaveOrderAsync_OrderStatusMissingItems_SaveOrderAsJsonInMissingSubFolder()
    {
        var s3Client = new AmazonS3Client();

        var settings = new ExternalServicesSettings
        {
            OrderBucketName = BucketName
        };
        var target = new OrderRepository(s3Client,settings);

        var orderName = $"order-{Guid.NewGuid()}";
        var order = new Order(orderName, "customer-1", "address-1", new[]
        {
            new OrderItem("product-1", ItemStatus.NotInInventory)
        });

        await target.SaveOrderAsync(order);

        var expectedOrder = $"MissingItems/{orderName}";
        var getObjectResponse = await s3Client.GetObjectAsync(BucketName, expectedOrder);

        await using var stream = getObjectResponse.ResponseStream;
        using var streamReader = new StreamReader(stream);

        var actualJson = await streamReader.ReadToEndAsync();
        var actual = JsonSerializer.Deserialize<Order>(actualJson);
        
        Assert.That(actual, Is.EqualTo(order));
    }
    
    [Test]
    public async Task SaveOrderAsync_OrderWithoutAnyItems_SaveOrderAsJsonInNoItemsInOrder()
    {
        var s3Client = new AmazonS3Client();

        var settings = new ExternalServicesSettings
        {
            OrderBucketName = BucketName
        };
        var target = new OrderRepository(s3Client,settings);

        var orderName = $"order-{Guid.NewGuid()}";
        var order = new Order(orderName, "customer-1", "address-1", Array.Empty<OrderItem>());

        await target.SaveOrderAsync(order);

        var expectedOrder = $"NoItemsInOrder/{orderName}";
        var getObjectResponse = await s3Client.GetObjectAsync(BucketName, expectedOrder);

        await using var stream = getObjectResponse.ResponseStream;
        using var streamReader = new StreamReader(stream);

        var actualJson = await streamReader.ReadToEndAsync();
        var actual = JsonSerializer.Deserialize<Order>(actualJson);
        
        Assert.That(actual, Is.EqualTo(order));
    }
}