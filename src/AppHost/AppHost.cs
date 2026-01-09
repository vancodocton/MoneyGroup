var builder = DistributedApplication.CreateBuilder(args);

var mssql = builder.AddSqlServer("mssql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("..\\Infrastructure.SqlServer\\Docker\\scripts", "/mssql-server-setup-scripts.d/")
    .AddDatabase("SqlServerConnection", "MoneyGroup");

builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi")
    .WithReference(mssql);

await builder.Build().RunAsync();
