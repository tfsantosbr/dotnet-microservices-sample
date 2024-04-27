using Basket.Api.Configurations.HealthCheck;
using Basket.Api.Models;
using Basket.Api.Models.Validators;
using Eventflix.Api.Extensions.Configurations;
using FluentValidation;
using FluentValidation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Open Telemetry =========================================================================================


//const string serviceName = "basket-api";

// builder.Logging.AddOpenTelemetry(options =>
// {
//     options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
//         .AddConsoleExporter();
// });

// builder.Services.AddOpenTelemetry()
//       .ConfigureResource(resource => resource.AddService(serviceName))
//       .WithTracing(tracing => tracing
//           .AddAspNetCoreInstrumentation()
//           .AddHttpClientInstrumentation()
//           .AddRedisInstrumentation()
//           .AddConsoleExporter())
//       .WithMetrics(metrics => metrics
//           .AddAspNetCoreInstrumentation()
//           .AddRuntimeInstrumentation()
//           .AddHttpClientInstrumentation()
//           .AddConsoleExporter()
//           .AddPrometheusExporter(options => 
//             options.DisableTotalNameSuffixForCounters = true)
//         );

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter()
    )
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter()
    );

builder.Logging.AddOpenTelemetry(logging => logging
        .AddOtlpExporter()
    );

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

// add logs
builder.Host.AddLogs(builder.Configuration);

var app = builder.Build();

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
