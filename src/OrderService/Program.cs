using System.Runtime.CompilerServices;
using OrderService;

[assembly: InternalsVisibleTo("OrderServiceTests")]

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

await host.RunAsync();