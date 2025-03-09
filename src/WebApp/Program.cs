using MoneyGroup.WebApi;
using MoneyGroup.WebApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ----------- Add this part to register the generated client -----------
// Add Kiota handlers to the dependency injection container
builder.Services.AddKiotaHandlers();

// Register the factory for the GitHub client
builder.Services.AddHttpClient<WebApiClientFactory>((sp, client) =>
{
    // Set the base address and accept header
    // or other settings on the http client
    client.BaseAddress = new Uri("https://localhost:7248");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
}).AttachKiotaHandlers(); // Attach the Kiota handlers to the http client, this is to enable all the Kiota features.

// Register the GitHub client
builder.Services.AddTransient(sp => sp.GetRequiredService<WebApiClientFactory>().GetClient());
// ----------- Add this part to register the generated client end -------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

await app.RunAsync();