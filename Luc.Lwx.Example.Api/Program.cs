using Luc.Lwx.Example.Api.Generated;
using Luc.Lwx.LwxActivityLog;
using Luc.Lwx.LwxConfig;
using Luc.Lwx.LwxCors;
using Luc.Lwx.LwxSetupSwagger;
using Luc.Lwx.LwxWaitForServerStartFix;

var builder = WebApplication.CreateBuilder(args);

builder.LwxConfigureSwagger(
    title: "Luc.Lwx.Example.Api",
    description: "This is an example API for Luc.Lwx",
    contactEmail: "lucas@example.com",
    author: "Lucas",
    version: "v1",
    additionalUrls: [ "https://apis.example.com" ]
);
builder.LwxConfigureCors();
builder.LwxLoadConfig();
builder.LwxConfigureActivityLog();
builder.LwxConfigureActivityLogOutput(new LwxActivityLogTestOutput() { });
builder.MapAuthSchemes_LucLwxExampleApi();
builder.MapAuthPolicies_LucLwxExampleApi();
builder.LwxConfigureHealthCheckFix();

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();
app.LwxConfigureSwagger();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.LwxConfigureActivityLog();
app.LwxConfigureHealhCheckFix();

app.MapGet("/", () => "Hello World!");
app.MapEndpoints_LucLwxExampleApi();
app.MapSwagger();
app.MapHealthChecks("/healthz");

await app.RunAsync();
