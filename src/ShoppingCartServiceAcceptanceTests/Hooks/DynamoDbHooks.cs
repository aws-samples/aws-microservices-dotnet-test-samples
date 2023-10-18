using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.TestUtils.DataAccess;

namespace ShoppingCartServiceAcceptanceTests.Hooks
{
    [Binding]
    public class DynamoDbHooks
    {
        private const string ShoppingCartsTableName = "ShoppingCarts";
        private static DynamoDbRunner? _dynamoDbRunner;
        
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _dynamoDbRunner = new DynamoDbRunner();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _dynamoDbRunner?.Dispose();
            _dynamoDbRunner = null;
        }
        
        [BeforeScenario]
        public void CreateTables()
        {
            var createTableRequest = new CreateTableRequest
            {
                TableName = ShoppingCartsTableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new("Id", "S")
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new("Id", KeyType.HASH)
                },
                ProvisionedThroughput = new ProvisionedThroughput{ReadCapacityUnits = 1, WriteCapacityUnits = 1}
            };
             _dynamoDbRunner?.Client.CreateTableAsync(createTableRequest).Wait();
        }

        [AfterScenario]
        public void DeleteTables()
        {
            _dynamoDbRunner?.Client.DeleteTableAsync(ShoppingCartsTableName).Wait();
        }
    }
}