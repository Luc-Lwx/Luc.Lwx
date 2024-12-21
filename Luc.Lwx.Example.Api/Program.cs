
using Luc.Lwx.Example.Api.Generated;
using Luc.Lwx.LwxActivityLog;

var builder = WebApplication.CreateBuilder(args);

builder.MapAuthSchemes_LucLwxExampleApi();
builder.MapAuthPolicies_LucLwxExampleApi();


var app = builder.Build();
app.UseLwxActivityLog();

app.MapGet("/", () => "Hello World!");
app.MapEndpoints_LucLwxExampleApi();

await app.RunAsync();
