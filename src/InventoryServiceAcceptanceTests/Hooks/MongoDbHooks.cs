using BoDi;
using Common.TestUtils.DataAccess;
using MongoDB.Driver;

namespace InventoryServiceAcceptanceTests.Hooks;

[Binding]
public class MongoDbHooks
{
    private const int MongoOutPort = 1111;
    public const string DatabaseName = "inventoryDb";
    private static MongoDbRunner _mongoDbRunner = null!;
    private readonly IObjectContainer _objectContainer;

    public MongoDbHooks(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        _mongoDbRunner = new MongoDbRunner(MongoOutPort);
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        _mongoDbRunner.Dispose();
    }

    [BeforeScenario]
    public void CleanupDatabase()
    {
        _objectContainer.RegisterInstanceAs(_mongoDbRunner);

        var client = new MongoClient(_mongoDbRunner.ConnectionString);
        client.DropDatabase(DatabaseName);
    }
}