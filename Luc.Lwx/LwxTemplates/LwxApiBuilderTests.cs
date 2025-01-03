using System.Text.Json.Serialization.Metadata;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Luc.Lwx.LwxConfig;
using Luc.Lwx.LwxCors;
using Luc.Lwx.LwxJsonOptions;
using Luc.Lwx.LwxSetupSwagger;
using Luc.Lwx.LwxStartupFix;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Diagnostics.Metrics;


namespace Luc.Lwx.LwxTemplates;

public class LwxApiBuilderTests : IHostApplicationBuilder
{
    private readonly WebApplicationBuilder _builder;
    private readonly List<IJsonTypeInfoResolver> _typeInfoResolvers = [];

    public LwxApiBuilderTests
    (
        string[] args
    )
    {
        _builder = WebApplication.CreateSlimBuilder(args);
        _builder.Configuration.AddEnvironmentVariables();

        _builder.Services.Configure<RouteOptions>(options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

        _builder.LwxConfigureCors();
        _builder.LwxLoadConfig();
        _builder.LwxConfigureActivityLog();
        _builder.LwxConfigureStartupFix();

        _builder.Services.AddEndpointsApiExplorer();
    }


    public async Task<WebApplication> Build()
    {
        if( _typeInfoResolvers.Count > 0 )
        {
            _builder.LwxAddJsonTypeResolvers( _typeInfoResolvers.ToArray() );
        }
        else 
        {
            throw new LwxConfigException("No JsonTypeInfoResolver added to the builder");
        }

        var app = _builder.Build();
        app.UseRouting();
        app.LwxConfigureActivityLog();
        app.LwxConfigureStartupFix();

        app.LwxConfigureSwagger();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSwagger();
        app.MapHealthChecks("/healthz");

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public void LwxAddTypeInfoResolver(IJsonTypeInfoResolver resolver)
    {
        _typeInfoResolvers.Add(resolver);
    }

    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        ((IHostApplicationBuilder)_builder).ConfigureContainer(factory, configure);
    }

    public IConfigurationManager Configuration => ((IHostApplicationBuilder)_builder).Configuration;

    public IHostEnvironment Environment => ((IHostApplicationBuilder)_builder).Environment;

    public ILoggingBuilder Logging => _builder.Logging;

    public IMetricsBuilder Metrics => _builder.Metrics;

    public IDictionary<object, object> Properties => ((IHostApplicationBuilder)_builder).Properties;

    public IServiceCollection Services => _builder.Services;
}