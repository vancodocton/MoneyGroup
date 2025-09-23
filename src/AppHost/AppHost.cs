var builder = DistributedApplication.CreateBuilder(args);

var mssqlServer = builder.AddSqlServer("sqlserver")
    .WithBindMount("../Infrastructure.SqlServer/Docker/scripts/", "/mssql-server-setup-scripts.d/")
    .WithLifetime(ContainerLifetime.Persistent);

var mssqlDatabase = mssqlServer.AddDatabase("SqlServerConnection", "MoneyGroup");

var webapi = builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi")
    .WithReference(mssqlDatabase).WaitFor(mssqlDatabase);

await builder.Build().RunAsync();
