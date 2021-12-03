using Elastic.Apm.NetCoreAll;
using Eventflix.Api.Extensions.Configurations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Users.Idp.Infrastructure.Context;
using Users.Idp.Models;
using Users.Idp.Models.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add validations
builder.Services.AddFluentValidation();
builder.Services.AddTransient<IValidator<CreateAccount>, CreateAccountValidator>();
builder.Services.AddTransient<IValidator<SignIn>, SignInValidator>();

// add context
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
    );

// Add Logs
builder.Host.AddLogs(builder.Configuration);
builder.Host.UseAllElasticApm();

var app = builder.Build();

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetService<UsersDbContext>();
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
