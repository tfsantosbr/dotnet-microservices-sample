using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.Consumer;
using Orders.Consumer.Configurations;
using Orders.Consumer.Configurations.HealthCheck.Publishers;
using Orders.Consumer.Repositories;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddTransient<OrderRepository>();

        // health check

        services.AddHealthChecks()
            .AddMongoDb(configuration.GetSection("OrdersDatabase:ConnectionString").Value)
            .AddKafka(new ProducerConfig() { BootstrapServers = configuration.GetSection("kafka:BootstrapServers").Value })
            ;

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(5);
            options.Period = TimeSpan.FromSeconds(5);
        });

        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        services.AddHostedService<Worker>();
    })
    .AddLogs()
    .Build();

await host.RunAsync();
