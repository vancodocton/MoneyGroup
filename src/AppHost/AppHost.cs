#pragma warning disable ASPIRECOMPUTE003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var builder = DistributedApplication.CreateBuilder(args);

var ghcr = builder.AddContainerRegistry("ghcr", "ghcr.io", repository: "vancodocton");

var mssql = builder.AddSqlServer("mssql", port: 1435)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("../Infrastructure.SqlServer/Docker/scripts", "/mssql-server-setup-scripts.d/")
    .AddDatabase("SqlServerConnection", "MoneyGroup");

var webapi = builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi")
    .WithContainerRegistry(ghcr)
    .WaitFor(mssql)
    .WithReference(mssql);

builder.AddViteApp("moneygroup-clientapp", "../ClientApp")
    .WithContainerRegistry(ghcr)
    .WithReference(webapi)
    .WaitFor(webapi);

await builder.Build().RunAsync();
