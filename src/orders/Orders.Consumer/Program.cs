using Elastic.Apm.NetCoreAll;
using Orders.Consumer;
using Orders.Consumer.Repositories;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<OrderRepository>();

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
