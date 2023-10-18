using Common.TestUtils.DataAccess;
using InventoryService.Config;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;

namespace InventoryServiceTests;

public class MongoDbTestBase
{
    private const string TestDatabaseName = "TestDb";
    private MongoDbRunner? _mongoDb;

    [OneTimeSetUp]
    public void InitMongo()
    {
        _mongoDb = new MongoDbRunner();
    }

    [OneTimeTearDown]
    public void DisposeDb()
    {
        _mongoDb?.Dispose();
        _mongoDb = null;
    }
     
    [TearDown]
    public void CleanDb()
    {
        var client = new MongoClient(MongoDbRunner.ConnectionString);
        client.DropDatabase(TestDatabaseName);
    }

    protected static IInventoryDatabaseSettings GetDatabaseSettings()
    {
        return new InventoryDatabaseSettings()
        {
            DatabaseName = TestDatabaseName,
            ConnectionString = MongoDbRunner.ConnectionString,
        };
    }
}