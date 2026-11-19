var builder = DistributedApplication.CreateBuilder(args);

var mssql = builder.AddSqlServer("mssql", port: 1435)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("../Infrastructure.SqlServer/Docker/scripts", "/mssql-server-setup-scripts.d/")
    .AddDatabase("SqlServerConnection", "MoneyGroup");

var webapi = builder.AddDotnetProject("moneygroup-webapi", "../WebApi/MoneyGroup.WebApi.csproj")
    .WithHttpHealthCheck("/health")
    .WaitFor(mssql)
    .WithReference(mssql);

builder.AddViteApp("moneygroup-clientapp", "../ClientApp")
    .WithReference(webapi)
    .WaitFor(webapi);

await builder.Build().RunAsync();
