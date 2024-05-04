using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.Consumer;
using Orders.Consumer.Configurations;
using Orders.Consumer.Configurations.HealthCheck.Publishers;
using Orders.Consumer.Metrics;
using Orders.Consumer.Repositories;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        
        services.AddTransient<OrderRepository>();

        // OPEN TELEMETRY =========================================================================================

        const string serviceName = "orders-consumer";

        // Metrics Class

        services.AddSingleton<OrdersConsumerMetrics>();

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddSource(OrdersConsumerMetrics.ActivitySourceName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri("http://otel-collector:4317")
                )
            )
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddMeter(OrdersConsumerMetrics.MeterName)
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri("http://otel-collector:4317")
                )
            );

        // builder.Logging.AddOpenTelemetry(logging => logging
        //         .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        //         .AddOtlpExporter(options =>
        //             options.Endpoint = new Uri("http://otel-collector:4317")
        //         )
        //     );

        // =======================================================================================================

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
