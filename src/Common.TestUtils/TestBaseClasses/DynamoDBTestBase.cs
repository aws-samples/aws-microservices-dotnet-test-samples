using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.TestUtils.DataAccess;
using NUnit.Framework;

namespace Common.TestUtils.TestBaseClasses;

public class DynamoDbTestBase
{
    private const string ShoppingCartsTableName = "ShoppingCarts";
    private const int ExternalPort = 8111;
    private DynamoDbRunner? _dynamoDbRunner;

    protected IAmazonDynamoDB Client
    {
        get
        {
            if (_dynamoDbRunner == null) Assert.Fail(nameof(_dynamoDbRunner) + " != null");

            return _dynamoDbRunner!.Client;
        }
    }

    [OneTimeSetUp]
    public void InitDb()
    {
        _dynamoDbRunner = new DynamoDbRunner(ExternalPort);
    }

    [OneTimeTearDown]
    public void DisposeDb()
    {
        _dynamoDbRunner?.Dispose();
        _dynamoDbRunner = null;
    }

    [SetUp]
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
            ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 1, WriteCapacityUnits = 1 }
        };
        _dynamoDbRunner?.Client.CreateTableAsync(createTableRequest).Wait();
    }

    [TearDown]
    public void DropTables()
    {
        _dynamoDbRunner?.Client.DeleteTableAsync(ShoppingCartsTableName).Wait();
    }
}