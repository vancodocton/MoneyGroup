using Aspire.Hosting.Docker;

var builder = DistributedApplication.CreateBuilder(args);

var mssql = builder.AddSqlServer("mssql", port: 1435)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("../Infrastructure.SqlServer/Docker/scripts", "/mssql-server-setup-scripts.d/")
    .AddDatabase("SqlServerConnection", "MoneyGroup");

var webapi = builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi")
    .WaitFor(mssql)
    .WithReference(mssql);

if (builder.ExecutionContext.IsPublishMode)
{
    builder.AddDockerComposeEnvironment("env");

    var registryEndpoint = builder.AddParameterFromConfiguration("registryEndpoint", "REGISTRY_ENDPOINT");

#pragma warning disable ASPIRECOMPUTE003, ASPIREPIPELINES003
    var registry = builder.AddContainerRegistry("ghcr", registryEndpoint);

    webapi
        .PublishAsDockerComposeService((_, service) => service.Name = "moneygroup-webapi")
        .WithContainerRegistry(registry)
        .WithImagePushOptions(ctx =>
        {
            ctx.Options.RemoteImageName = (
                Environment.GetEnvironmentVariable("REGISTRY_REPOSITORY") ?? "vancodocton/moneygroup"
            ).ToLowerInvariant();
            ctx.Options.RemoteImageTag = Environment.GetEnvironmentVariable("IMAGE_TAG") ?? "latest";
        });
#pragma warning restore ASPIRECOMPUTE003, ASPIREPIPELINES003
}

builder.AddViteApp("moneygroup-clientapp", "../ClientApp")
    .WithReference(webapi)
    .WaitFor(webapi);

await builder.Build().RunAsync();
