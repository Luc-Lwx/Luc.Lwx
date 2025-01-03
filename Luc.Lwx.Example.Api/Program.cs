using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using Luc.Lwx;
using Luc.Lwx.Example.Api.Generated;
using Luc.Lwx.LwxActivityLog;
using Luc.Lwx.LwxConfig;
using Luc.Lwx.LwxCors;
using Luc.Lwx.LwxJsonOptions;
using Luc.Lwx.LwxSetupSwagger;
using Luc.Lwx.LwxStartupFix;
using Luc.Lwx.LwxTemplates;
using Luc.Lwx.Generated;
using Microsoft.AspNetCore.Http.Json;

var builder = new LwxApiBuilder(args);

builder.LwxAddTypeInfoResolver( Luc.Lwx.Example.Api.SourceGenerationContext.Default );
builder.LwxAddTypeInfoResolver( Luc.Lwx.SourceGenerationContext.Default );    

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
builder.LwxConfigureActivityLogOutput(new LwxActivityLogTestOutput() { });

builder.MapAuthSchemes_LucLwxExampleApi();
builder.MapAuthPolicies_LucLwxExampleApi();

var app = await builder.Build();

app.MapEndpoints_LucLwxExampleApi();

await app.RunAsync();
