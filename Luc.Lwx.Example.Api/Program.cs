using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using Luc.Lwx;
using Luc.Lwx.Example.Api.Generated;
using Luc.Lwx.LwxActivityLog;
using Luc.Lwx.LwxConfig;
using Luc.Lwx.LwxCors;
using Luc.Lwx.LwxSetupSwagger;
using Luc.Lwx.LwxStartupFix;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine
    (
        Luc.Lwx.Example.Api.SourceGenerationContext.Default,
        Luc.Lwx.SourceGenerationContext.Default
    );
});



builder.LwxConfigureSwagger(
    title: "Luc.Lwx.Example.Api",
    description: "This is an example API for Luc.Lwx",
    contactEmail: "lucas@example.com",
    author: "Lucas",
    version: "v1",
    additionalUrls: [ 
        "https://apis.example.com" 
    ]
);
builder.LwxConfigureCors();
builder.LwxLoadConfig();
builder.LwxConfigureActivityLog();
builder.LwxConfigureActivityLogOutput(new LwxActivityLogTestOutput() { });
builder.MapAuthSchemes_LucLwxExampleApi();
builder.MapAuthPolicies_LucLwxExampleApi();
builder.LwxConfigureStartupFix();

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();
app.LwxConfigureSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.LwxConfigureActivityLog();
app.LwxConfigureStartupFix();

app.MapEndpoints_LucLwxExampleApi();
app.MapSwagger();
app.MapHealthChecks("/healthz");

await app.RunAsync();
