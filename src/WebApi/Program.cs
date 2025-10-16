using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Services;
using MoneyGroup.Core.Validators;
using MoneyGroup.Infrastructure.Data;
using MoneyGroup.Infrastructure.Mapperly;
using MoneyGroup.Infrastructure.SqlServer;
using MoneyGroup.WebApi.Authorizations;
using MoneyGroup.WebApi.Endpoints;
using MoneyGroup.WebApi.Middlewares;
using MoneyGroup.WebApi.Validators;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks();

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        // Configuration is loaded from appsettings.json
    });

builder.Services.AddAuthorizationBuilder()
    .AddDefaultPolicy("DefaultPolicy", policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireAuthorizedUser();
    });
builder.Services.AddScoped<IAuthorizationHandler, DenyUnauthorizedUserHandler>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.Encoder = null;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    var metadataAddress = builder.Configuration[$"Authentication:Schemes:{JwtBearerDefaults.AuthenticationScheme}:MetadataAddress"];
    var validIssuer = builder.Configuration[$"Authentication:Schemes:{JwtBearerDefaults.AuthenticationScheme}:ValidIssuer"];

    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes.Add("bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header,
            BearerFormat = "Json Web Token",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
        });

        if (!string.IsNullOrWhiteSpace(metadataAddress) && validIssuer != "dotnet-user-jwts")
        {
            document.Components.SecuritySchemes.Add("google-oidc", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                In = ParameterLocation.Header,
                OpenIdConnectUrl = new Uri(metadataAddress),
                BearerFormat = "Json Web Token",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });
        }

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", context.Document)] = []
        });
        if (!string.IsNullOrWhiteSpace(metadataAddress))
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("google-oidc", context.Document)] = []
            });
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddMapper();

var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection")
    ?? throw new InvalidOperationException();
builder.Services.AddApplicationDbContextSqlServer(connectionString);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddProblemDetails();

#region Validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSingleton<IValidator<IPaginatedOptions>, PaginatedOptionsValidator>();
builder.Services.AddSingleton<IValidator<OrderPaginatedRequest>, OrderPaginatedRequestValidator>();
builder.Services.AddSingleton<IValidator<UserPaginatedRequest>, UserPaginatedRequestValidator>();
builder.Services.AddSingleton<IValidator<ParticipantDto>, ParticipantDtoValidator>();
builder.Services.AddSingleton<IValidator<OrderDto>, OrderDtoValidator>();
builder.Services.AddExceptionHandler<BusinessValidationExceptionHandler>();
#endregion Validators

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "MoneyGroup API V1");
    });
}
else
{
    app.UseStatusCodePages();
}

app.UseExceptionHandler();

app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapOrderEndpoints();
app.MapUserEndpoints();

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
public sealed partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors
