[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

var builder = DistributedApplication.CreateBuilder(args);

var mssql = builder.AddSqlServer("mssql", port: 1435)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("../Infrastructure.SqlServer/Docker/scripts", "/mssql-server-setup-scripts.d/")
    .AddDatabase("SqlServerConnection", "MoneyGroup");

builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi")
    .WaitFor(mssql)
    .WithReference(mssql);

await builder.Build().RunAsync();
