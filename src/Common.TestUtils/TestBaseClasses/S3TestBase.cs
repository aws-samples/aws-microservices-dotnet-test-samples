using Amazon.S3;
using Common.TestUtils.DataAccess;
using NUnit.Framework;

namespace Common.TestUtils.TestBaseClasses;

public class S3TestBase
{
    private const string BucketNamePrefix = "test-order-bucket";
    protected string BucketName { get; private set; } = null!;

    private S3TestRunner? _s3TestRunner;
    
    [OneTimeSetUp]
    public void CreateBucket()
    {
        BucketName = $"{BucketNamePrefix}-{Guid.NewGuid()}";

        _s3TestRunner = new S3TestRunner(BucketName);
    }

    [OneTimeTearDown]
    public void DeleteBucket()
    {
        _s3TestRunner?.Dispose();
        _s3TestRunner = null;
    }

    protected async Task DeleteAllFilesAsync()
    {
        if (_s3TestRunner != null) 
            await _s3TestRunner.DeleteAllFilesAsync();
    }
}