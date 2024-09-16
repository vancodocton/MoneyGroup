using Microsoft.EntityFrameworkCore;

using MoneyGroup.Infrastucture.AutoMapper.Profiles;
using MoneyGroup.Infrastucture.PostgreSql;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
public sealed partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors