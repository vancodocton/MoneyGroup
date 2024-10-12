using Microsoft.EntityFrameworkCore;

using MoneyGroup.Infrastucture.Data;
using MoneyGroup.Infrastucture.PostgreSql;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection")
    ?? throw new InvalidOperationException();
builder.Services.AddApplicationDbContextNpgsql(connectionString);

var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

await dbContext.Database.MigrateAsync();