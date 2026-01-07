var builder = DistributedApplication.CreateBuilder(args);

await builder.Build().RunAsync();
