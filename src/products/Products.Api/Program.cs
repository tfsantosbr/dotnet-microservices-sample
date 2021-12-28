using Elastic.Apm.EntityFrameworkCore;
using Elastic.Apm.NetCoreAll;
using Eventflix.Api.Extensions.Configurations;
using Microsoft.EntityFrameworkCore;
using Products.Api.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Context configuration
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlServer"))
    );

// Add Logs
builder.Host.AddLogs(configuration);
builder.Host.UseAllElasticApm());

var app = builder.Build();

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetService<ProductsDbContext>();
context?.Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
