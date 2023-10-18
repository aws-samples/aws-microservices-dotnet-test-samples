using System.Diagnostics;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;

namespace Common.TestUtils.DataAccess;

public class MongoDbRunner : IDisposable
{
    public int MongoOutPort { get; }
    private const string ImageName = "mongo_test";
    private const string MongoInPort = "27017";
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(60);
    
    private static Process? _process;

    public string ConnectionString => $"mongodb://localhost:{MongoOutPort}";

    public MongoDbRunner(int mongoOutPort)
    {
        MongoOutPort = mongoOutPort;
        _process = Process.Start("docker", $"run --name {ImageName} -p {MongoOutPort}:{MongoInPort} mongo");
        var started = WaitForMongoDbConnection(ConnectionString, "admin");
        if (!started)
            throw new Exception(
                $"Startup failed, could not get MongoDB connection after trying for '{TestTimeout}'");
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
    
    private static bool WaitForMongoDbConnection(string connectionString, string dbName)
    {
        Console.Out.WriteLine("Waiting for Mongo to respond");
        var probeTask = Task.Run(() =>
        {
            var isAlive = false;
            var client = new MongoClient(connectionString);

            for (var i = 0; i < 3000; i++)
            {
                client.GetDatabase(dbName);
                var server = client.Cluster.Description.Servers.FirstOrDefault();
                isAlive = server != null &&
                          server.HeartbeatException == null &&
                          server.State == ServerState.Connected;

                if (isAlive) break;

                Thread.Sleep(100);
            }

            return isAlive;
        });
        probeTask.Wait();
        return probeTask.Result;
    }

}