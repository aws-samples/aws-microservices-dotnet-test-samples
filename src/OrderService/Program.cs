using System.Runtime.CompilerServices;
using OrderService.Extensions;

[assembly: InternalsVisibleTo("OrderServiceTests")]
[assembly: InternalsVisibleTo("OrderServiceAcceptanceTests")]

var host = Host.CreateDefaultBuilder(args)
    .ConfigureWorkerServices()
    .Build();

await host.RunAsync();

// Enable using Program in Acceptance tests
#pragma warning disable CA1050
public partial class Program
{
}
#pragma warning restore CA1050