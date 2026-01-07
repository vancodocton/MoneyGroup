var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MoneyGroup_WebApi>("moneygroup-webapi");

await builder.Build().RunAsync();
