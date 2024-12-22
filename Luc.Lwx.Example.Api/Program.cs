using Luc.Lwx.Example.Api.Generated;
using Luc.Lwx.LwxActivityLog;
using Luc.Lwx.LwxConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.RequireLwxDevConfig();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.SetLwxActivityLogConfig(new LwxActivityLogConfig() { });
builder.Services.SetLwxActivityLogOutput(new LwxActivityLogTestOutput() { });
builder.MapAuthSchemes_LucLwxExampleApi();
builder.MapAuthPolicies_LucLwxExampleApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseLwxActivityLog();

app.MapGet("/", () => "Hello World!");
app.MapEndpoints_LucLwxExampleApi();
app.MapSwagger();

await app.RunAsync();
