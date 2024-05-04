using Confluent.Kafka;
using Eventflix.Api.Extensions.Configurations;
using Orders.Api.Metrics;
using Orders.Api.Repositories;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<OrderRepository>();
builder.Services.AddSingleton<OrdersApiMetrics>();

// OPEN TELEMETRY =========================================================================================

const string serviceName = "orders-api";

// Metrics Class

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddSource(OrdersApiMetrics.ActivitySourceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://otel-collector:4317")
        )
    )
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddMeter(OrdersApiMetrics.MeterName)
        .AddAspNetCoreInstrumentation()
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

// add health check
builder.Services.AddHealthChecks()
    .AddKafka(new ProducerConfig() { BootstrapServers = configuration.GetSection("kafka:BootstrapServers").Value })
    .AddMongoDb(configuration.GetSection("OrdersDatabase:ConnectionString").Value)
    ;

// Add Logs
builder.Host.AddLogs(builder.Configuration);

var app = builder.Build();

// map health check
app.MapHealthChecks("/healthz");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
