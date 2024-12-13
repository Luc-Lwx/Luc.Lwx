
using Luc.Web.Example.Api.Generated;

var builder = WebApplication.CreateBuilder(args);

builder.MapAuthSchemes_LucWebExampleApi();
builder.MapAuthPolicies_LucWebExampleApi();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapEndpoints_LucWebExampleApi();

await app.RunAsync();
