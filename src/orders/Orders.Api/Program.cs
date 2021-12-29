using Confluent.Kafka;
using Elastic.Apm.NetCoreAll;
using Eventflix.Api.Extensions.Configurations;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add health check
builder.Services.AddHealthChecks()
    .AddKafka(new ProducerConfig() { BootstrapServers = configuration.GetSection("kafka:BootstrapServers").Value })
    ;

// Add Logs
builder.Host.AddLogs(builder.Configuration);
builder.Host.UseAllElasticApm();

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
