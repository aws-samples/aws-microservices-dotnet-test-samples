using Common.TestUtils.DataAccess;
using MongoDB.Driver;
using NUnit.Framework;

namespace Common.TestUtils.TestBaseClasses;

public class MongoDbTestBase
{
    private const int MongoOutPort = 2222;
    private static MongoDbRunner? _mongoDbRunner;
    protected string TestDatabaseName => "TestDb";
    protected string ConnectionString => _mongoDbRunner!.ConnectionString;

    [OneTimeSetUp]
    public static void InitMongo()
    {
        _mongoDbRunner = new MongoDbRunner(MongoOutPort);
    }

    [OneTimeTearDown]
    public static void DisposeDb()
    {
        _mongoDbRunner?.Dispose();
        _mongoDbRunner = null;
    }

    [TearDown]
    public void CleanDb()
    {
        var client = new MongoClient(_mongoDbRunner!.ConnectionString);
        client.DropDatabase(TestDatabaseName);
    }
}