var builder = DistributedApplication.CreateBuilder(args);

var mssql = builder.AddSqlServer("mssql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("SqlServerConnection");

builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi")
    .WithReference(mssql);

await builder.Build().RunAsync();
