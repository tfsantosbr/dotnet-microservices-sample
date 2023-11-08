using Confluent.Kafka;
using Elastic.Apm.Extensions.Hosting;
using Elastic.Apm.MongoDb;
using Elastic.Apm.SerilogEnricher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.Consumer;
using Orders.Consumer.Configurations.HealthCheck;
using Orders.Consumer.Configurations.HealthCheck.Publishers;
using Orders.Consumer.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

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
    .UseSerilog((context, provider) =>
    {
        var configuration = context.Configuration;

        var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri(configuration["Elasticsearch:Uri"]))
        {
            IndexFormat = configuration["Elasticsearch:IndexFormat"]
        };

        provider
            .MinimumLevel.Override("Elastic.Apm", LogEventLevel.Fatal)
            .Enrich.WithElasticApmCorrelationInfo()
            .Enrich.WithCorrelationId()
            .Enrich.WithMachineName()
            .Enrich.WithClientIp()
            .Enrich.WithClientAgent()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(elasticsearchSinkOptions);
    })
    .UseElasticApm(new MongoDbDiagnosticsSubscriber())
    .Build();

await host.RunAsync();
