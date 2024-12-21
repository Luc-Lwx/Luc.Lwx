
using Luc.Lwx.Example.Api.Generated;

var builder = WebApplication.CreateBuilder(args);

builder.MapAuthSchemes_LucLwxExampleApi();
builder.MapAuthPolicies_LucLwxExampleApi();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapEndpoints_LucLwxExampleApi();

await app.RunAsync();
