
using Luc.Lwx.Interface;
using Luc.Lwx.LwxConfig;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;

namespace Luc.Lwx.Azure;

public static class LwxAzureExtensions 
{  
    public static void AddLwxAzure
    (
        this IHostApplicationBuilder builder, 
        bool useAppInsightsOnProd, 
        bool usePrometheusOnProd,
        bool useAppInsightsOnDev = false,
        bool usePrometheusOnDev = false
    ) 
    {        
        if ((builder.Environment.IsProduction() && useAppInsightsOnProd) || (builder.Environment.IsDevelopment() && useAppInsightsOnDev))
        {
            var connectionString = builder.Configuration.LwxGet<string>(
                "ApplicationInsights:ConnectionString",
                isRequired: true
            );

            builder.Configuration.LwxGet<string>(
                "ApplicationInsights:InstrumentationKey",
                converter: (section) => 
                {
                    if (section.Get<string>() != null) throw new LwxConfigException("InstrumentationKey is not allowed in appsettings.json");
                    return connectionString;
                }
            );

            var enableAdaptiveSampling = builder.Configuration.LwxGet<bool>(
                "ApplicationInsights:EnableAdaptiveSampling", 
                isRequired: true
            );
            var maxTelemetryItemsPerSecond = builder.Configuration.LwxGet<int>(
                "ApplicationInsights:MaxTelemetryItemsPerSecond", 
                isRequired: true
            );
            var minSamplingPercentage = builder.Configuration.LwxGet<int>(
                "ApplicationInsights:MinSamplingPercentage", 
                isRequired: true                
            );
            var maxSamplingPercentage = builder.Configuration.LwxGet<int>(
                "ApplicationInsights:MaxSamplingPercentage", 
                isRequired: true
            );

            builder.Configuration.LwxValidKeys
            (
                "ApplicationInsights", 
                [            
                    "ConnectionString",
                    "EnableAdaptiveSampling",
                    "MaxTelemetryItemsPerSecond",
                    "MinSamplingPercentage",
                    "MaxSamplingPercentage"
                ]
            );

            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = connectionString;
                options.EnableAdaptiveSampling = enableAdaptiveSampling;
            });

            builder.Services.Configure<TelemetryConfiguration>((config) =>
            {
                var adaptiveSamplingTelemetryProcessor = config.DefaultTelemetrySink.TelemetryProcessors
                    .OfType<AdaptiveSamplingTelemetryProcessor>()
                    .FirstOrDefault();

                if (adaptiveSamplingTelemetryProcessor != null)
                {
                    adaptiveSamplingTelemetryProcessor.MaxTelemetryItemsPerSecond = maxTelemetryItemsPerSecond;
                    adaptiveSamplingTelemetryProcessor.MaxSamplingPercentage = maxSamplingPercentage;
                    adaptiveSamplingTelemetryProcessor.MinSamplingPercentage = minSamplingPercentage;
                }
            });
        }
    }
}