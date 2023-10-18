using Common.TestUtils.DataAccess;
using InventoryService.Config;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;

namespace InventoryServiceTests;

public class MongoDbTestBase
{
    private const string TestDatabaseName = "TestDb";
    private const int MongoOutPort = 2222;
    private static MongoDbRunner? _mongoDbRunner;

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

    protected IInventoryDatabaseSettings GetDatabaseSettings()
    {
        return new InventoryDatabaseSettings()
        {
            DatabaseName = TestDatabaseName,
            ConnectionString = _mongoDbRunner!.ConnectionString,
        };
    }
}