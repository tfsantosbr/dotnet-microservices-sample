using Elastic.Apm.NetCoreAll;
using Elastic.Apm.SerilogEnricher;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.Consumer;
using Orders.Consumer.Configurations.HealthCheck;
using Orders.Consumer.Configurations.HealthCheck.Publishers;
using Orders.Consumer.Repositories;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<OrderRepository>();

        // health check

        services.AddHealthChecks()
            .AddCheck<ExampleHealthCheck>("example-health-check");

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
            IndexFormat = configuration["Elasticsearch:LogsSettings:IndexFormat"]
        };

        provider
            .Enrich.WithElasticApmCorrelationInfo()
            .Enrich.WithCorrelationId()
            .Enrich.WithMachineName()
            .Enrich.WithClientIp()
            .Enrich.WithClientAgent()
            .Enrich.WithProperty("AppName", configuration["Elasticsearch:LogsSettings:AppName"])
            .WriteTo.Console()
            .WriteTo.Elasticsearch(elasticsearchSinkOptions);
    })
    .UseAllElasticApm()
    .Build();

await host.RunAsync();
