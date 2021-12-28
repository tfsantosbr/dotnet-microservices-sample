using Basket.Api.Models;
using Basket.Api.Models.Validators;
using Elastic.Apm.NetCoreAll;
using Elastic.Apm.StackExchange.Redis;
using Eventflix.Api.Extensions.Configurations;
using FluentValidation;
using FluentValidation.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add validations
builder.Services.AddFluentValidation();
builder.Services.AddTransient<IValidator<BasketModel>, BasketModelValidator>();
builder.Services.AddTransient<IValidator<BasketProductItemModel>, BasketProductItemModelValidator>();

// add redis
var multiplexer = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
multiplexer.UseElasticApm();
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

// add logs
builder.Host.AddLogs(builder.Configuration);
builder.Host.UseAllElasticApm();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
