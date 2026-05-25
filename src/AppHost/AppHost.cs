#pragma warning disable ASPIRECOMPUTE003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable ASPIREPIPELINES003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable ASPIREJAVASCRIPT001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Aspire.Hosting.Azure.AppContainers;

using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var ghcr = builder
    .AddContainerRegistry("ghcr", "ghcr.io", repository: "vancodocton");

var acEnv = builder.AddAzureContainerAppEnvironment("moneygroup-env");

var mssql = builder.AddSqlServer("mssql", port: 1435)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("../Infrastructure.SqlServer/Docker/scripts", "/mssql-server-setup-scripts.d/")
    .AddDatabase("SqlServerConnection", "MoneyGroup");

var webapi = builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi")
    .WithContainerRegistry(ghcr)
    .WaitFor(mssql)
    .WithReference(mssql);

// TODO: Fix bug in https://github.com/vancodocton/MoneyGroup/actions/runs/26411668249/job/77747448911
if (builder.Environment.IsDevelopment())
{
    builder.AddViteApp("moneygroup-clientapp", "../ClientApp")
        .PublishAsStaticWebsite()
        .WithReference(webapi)
        .WaitFor(webapi);
}

await builder.Build().RunAsync();
