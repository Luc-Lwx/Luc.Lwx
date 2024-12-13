
using Luc.Util.Example.Api.Generated;

var builder = WebApplication.CreateBuilder(args);

builder.MapAuthSchemes_LucUtilExampleApi();
builder.MapAuthPolicies_LucUtilExampleApi();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapEndpoints_LucUtilExampleApi();

await app.RunAsync();
