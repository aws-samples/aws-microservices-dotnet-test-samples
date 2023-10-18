using System;
using System.Diagnostics;
using Common.TestUtils;
using Common.TestUtils.DataAccess;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using TechTalk.SpecFlow;

namespace InventoryServiceAcceptanceTests.Hooks
{
    [Binding]
    public class MongoDbHooks
    {
        public static readonly string DatabaseName = "inventoryDb";
        private static MongoDbRunner _mongo = null!;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _mongo = new MongoDbRunner();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _mongo.Dispose();
        }
        
        [BeforeScenario]
        public void CleanupDatabase()
        {
            var client = new MongoClient(MongoDbRunner.ConnectionString);
            client.DropDatabase(DatabaseName);
        }
    }
}