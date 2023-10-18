using System.Diagnostics;
using Amazon.DynamoDBv2;

namespace Common.TestUtils.DataAccess;

public class DynamoDbRunner : IDisposable
{
    public int ExternalPort { get; }
    private const string ImageName = "dynamoDbLocal_test";
        
    private Process? _process;

    public IAmazonDynamoDB Client { get; }

    public DynamoDbRunner(int externalPort)
    {
        ExternalPort = externalPort;

        _process = Process.Start("docker", $"run --name {ImageName} -p {ExternalPort}:8000 amazon/dynamodb-local");

        var clientConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = $"http://localhost:{ExternalPort}"
        };

        Client = new AmazonDynamoDBClient(clientConfig);
    }
    
    public void Dispose()
    {
        _process?.Dispose();
        _process = null;

        var processStop = Process.Start("docker", $"stop {ImageName}");
        processStop.WaitForExit();
        var processRm = Process.Start("docker", $"rm {ImageName}");
        processRm.WaitForExit();
    }
}