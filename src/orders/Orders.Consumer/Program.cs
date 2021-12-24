using Orders.Consumer;
using Orders.Consumer.Repositories;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<OrderRepository>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
