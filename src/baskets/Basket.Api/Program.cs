using System.Text.Json.Serialization;
using Basket.Api.Configurations.HealthCheck;
using Basket.Api.Metrics;
using Basket.Api.Models;
using Basket.Api.Models.Validators;
using Eventflix.Api.Extensions.Configurations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

// SERILOG ================================================================================================

builder.Services.AddSerilog((services, logger) => logger
    .ReadFrom.Configuration(configuration)
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithProcessName()
    .Enrich.WithExceptionDetails()
    .Enrich.WithClientIp()
    .Enrich.WithCorrelationId()
    .WriteTo.File(formatter: new JsonFormatter(), "log.json")
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = "http://host.docker.internal:4317";
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "basket-api"
        };
    })
);

// ========================================================================================================

// OPEN TELEMETRY =========================================================================================

const string serviceName = "basket-api";

// Metrics Class

builder.Services.AddSingleton<BasketMetrics>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddSource(BasketMetrics.ActivitySourceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRedisInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://otel-collector:4317")
        )
    )
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddMeter(BasketMetrics.MeterName)
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

// add validations
builder.Services.AddFluentValidation();
builder.Services.AddTransient<IValidator<BasketModel>, BasketModelValidator>();
builder.Services.AddTransient<IValidator<BasketProductItemModel>, BasketProductItemModelValidator>();

// add redis
var multiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

// add health check
builder.Services.AddHealthChecks()
    .AddCheck<RedisHealthCheck>("redis-health-check")
    ;

// add http
builder.Services.AddHttpClient();

// add logs
builder.Host.AddLogs(builder.Configuration);

var app = builder.Build();

// SERILOG ================================================================================================
app.UseSerilogRequestLogging();
// ========================================================================================================

// map health check
app.MapHealthChecks("/healthz");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
