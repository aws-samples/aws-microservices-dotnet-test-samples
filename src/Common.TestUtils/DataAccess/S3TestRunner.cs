using Amazon.S3;
using Amazon.S3.Model;

namespace Common.TestUtils.DataAccess;

public class S3TestRunner : IDisposable
{
    private readonly string _bucketName;
    private IAmazonS3 _s3Client;
    
    public S3TestRunner(string bucketName)
    {
        _bucketName = bucketName;
        _s3Client = new AmazonS3Client();
        _s3Client.PutBucketAsync(bucketName).Wait();
    }
    
    public void Dispose()
    {
        DeleteAllFilesAsync().Wait();
        _s3Client.DeleteBucketAsync(_bucketName).Wait();
    }

    public async Task DeleteAllFilesAsync()
    {
        var listObjectsResponse = await _s3Client.ListObjectsAsync(_bucketName);

        if (listObjectsResponse.S3Objects.Any() is false)
        {
            return;
        }
        
        await _s3Client.DeleteObjectsAsync(new DeleteObjectsRequest
        {
            BucketName =   _bucketName, 
            Objects = listObjectsResponse.S3Objects
                .Select(o => new KeyVersion{Key = o.Key})
                .ToList()
        });
    }
}