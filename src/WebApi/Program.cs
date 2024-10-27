using FluentValidation;

using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Services;
using MoneyGroup.Core.Validators;
using MoneyGroup.Infrastucture.AutoMapper.Profiles;
using MoneyGroup.Infrastucture.Data;
using MoneyGroup.Infrastucture.PostgreSql;
using MoneyGroup.WebApi.Endpoints;
using MoneyGroup.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper();

var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection")
    ?? throw new InvalidOperationException();
builder.Services.AddApplicationDbContextNpgsql(connectionString);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddProblemDetails();

#region Validators
builder.Services.AddSingleton<IValidator<ConsumerDto>, ConsumerDtoValidator>();
builder.Services.AddSingleton<IValidator<OrderDto>, OrderDtoValidator>();
builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
#endregion Validators

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

app.MapOrderEndpoints();

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
public sealed partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors