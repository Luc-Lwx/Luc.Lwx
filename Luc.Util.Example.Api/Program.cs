
using Luc.Util.Example.Api.Generated;

var builder = WebApplication.CreateBuilder(args);

builder.MapAuthPolicies();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapExampleEndpoints();



app.Run();
