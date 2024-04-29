using Eventflix.Api.Extensions.Configurations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Users.Idp.Domain;
using Users.Idp.Infrastructure.Context;
using Users.Idp.Infrastructure.Repositories;
using Users.Idp.Models;
using Users.Idp.Models.Validators;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add validations
builder.Services.AddFluentValidation();
builder.Services.AddTransient<IValidator<CreateAccount>, CreateAccountValidator>();
builder.Services.AddTransient<IValidator<SignIn>, SignInValidator>();

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// add health check
builder.Services.AddHealthChecks()
    .AddNpgSql(configuration.GetConnectionString("Postgres"))
    ;

// add context
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("Postgres"))
    );

// Add Logs
builder.Host.AddLogs(configuration);

var app = builder.Build();

// map health check
app.MapHealthChecks("/healthz");

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
