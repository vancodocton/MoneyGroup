using Microsoft.EntityFrameworkCore;

using MoneyGroup.Infrastructure.Data;
using MoneyGroup.Infrastructure.SqlServer;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection")
    ?? throw new InvalidOperationException();
builder.Services.AddApplicationDbContextSqlServer(connectionString);

var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

await dbContext.Database.MigrateAsync();