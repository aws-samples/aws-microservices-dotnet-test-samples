using Common.TestUtils.DataAccess;

namespace OrderServiceAcceptanceTests.Hooks
{
    [Binding]
    public class S3Hooks
    {
        public static readonly string S3OrderBucketName = $"test-order-bucket-{Guid.NewGuid().ToString()}";

        private static S3TestRunner? _s3TestRunner;
        
        [BeforeTestRun(Order = 0)]
        public static void BeforeTestRun()
        {
            _s3TestRunner = new S3TestRunner(S3OrderBucketName);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _s3TestRunner?.Dispose();
            _s3TestRunner = null;
        }

        [AfterScenario]
        public void DeleteBucketFiles()
        {
            _s3TestRunner?.DeleteAllFilesAsync().Wait();
        }
    }
}