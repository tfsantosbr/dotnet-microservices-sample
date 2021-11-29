using FluentValidation;
using FluentValidation.AspNetCore;
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
