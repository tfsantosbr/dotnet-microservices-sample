using Orders.Reporting;
using Orders.Reporting.Metrics;
using Orders.Reporting.Repositories;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<OrdersReportingWorker>();
builder.Services.AddTransient<OrderRepository>();
builder.Services.AddSingleton<OrdersReportMetrics>();

// OPEN TELEMETRY =========================================================================================

const string serviceName = "orders-reporting";

// Metrics Class

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddSource(OrdersReportMetrics.ActivitySourceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://otel-collector:4317")
        )
    )
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddMeter(OrdersReportMetrics.MeterName)
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

var host = builder.Build();
host.Run();
