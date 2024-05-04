using Eventflix.Api.Extensions.Configurations;
using Microsoft.EntityFrameworkCore;
using Products.Api.Infrastructure.Context;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Products.Api.Metrics;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ProductsApiMetrics>();

// OPEN TELEMETRY =========================================================================================

const string serviceName = "products-api";

// Metrics Class

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddSource(ProductsApiMetrics.ActivitySourceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
            options.Endpoint = new Uri("http://otel-collector:4317")
        )
    )
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddMeter(ProductsApiMetrics.MeterName)
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
    .AddSqlServer(configuration.GetConnectionString("SqlServer")!)
    ;

// Context configuration
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlServer"))
    );

// Add Logs
builder.Host.AddLogs(configuration);

var app = builder.Build();

// map health check
app.MapHealthChecks("/healthz");

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetService<ProductsDbContext>();
context?.Database.EnsureCreated();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
