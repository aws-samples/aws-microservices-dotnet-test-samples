using Amazon.S3;
using Common.TestUtils.DataAccess;
using NUnit.Framework;

namespace Common.TestUtils.TestBaseClasses;

public class S3TestBase
{
    private const string BucketNamePrefix = "test-order-bucket";
    protected string BucketName { get; private set; } = null!;
    
    [OneTimeSetUp]
    public void CreateQueue()
    {
        BucketName = $"{BucketNamePrefix}-{Guid.NewGuid()}";
        var s3Client = new AmazonS3Client();
        s3Client.PutBucketAsync(BucketName).Wait();
        
    }

    [OneTimeTearDown]
    public void DeleteQueue()
    {
        var s3Client = new AmazonS3Client();
        s3Client.DeleteBucketAsync(BucketName).Wait();
    }
}