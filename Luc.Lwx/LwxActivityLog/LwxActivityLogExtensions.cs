using System.Text.Json;
using Luc.Lwx.Interface;

namespace Luc.Lwx.LwxActivityLog;

public static class LwxActivityLogExtensions
{
    public static IApplicationBuilder LwxConfigureActivityLog(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LwxActivityLogMiddleware>();
    }

    public static WebApplicationBuilder LwxConfigureActivityLog(this WebApplicationBuilder builder)
    {        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
     
        var configSection = builder.Configuration.GetSection("LwxActivityLog") ?? throw new LwxConfigException("LwxActivityLog section is missing in the configuration file.");
        try
        {
            var config = configSection.Get<LwxActivityLogConfig>() ?? throw new LwxConfigException(GenerateConfigErrorMessage("Invalid format for LwxActivityLog section in the configuration file."));
            builder.Services.AddSingleton(config);            
        }
        catch (Exception ex)
        {
            throw new LwxConfigException(GenerateConfigErrorMessage("Error reading LwxActivityLog configuration section"), ex);
        }
        
        return builder;
    }

    public static WebApplicationBuilder LwxConfigureActivityLogOutput(this WebApplicationBuilder builder, ILwxActivityLogOutput output)
    {        
        builder.Services.AddTransient<LwxActivityLogMiddleware>();   
        builder.Services.AddSingleton<ILwxActivityLogOutput>(output);           
        return builder;
    }

    private const string s_operationRecordKey = "OperationRecord";

    internal static void SetLwxActivityRecord(this HttpContext context, LwxRecord record)
    {        
        context.Items[s_operationRecordKey] = record;
    }

    public static LwxRecord? GetLwxActivityRecord(this HttpContext context)
    {        
        return context.Items.TryGetValue(s_operationRecordKey, out var record) ? record as LwxRecord : null;
    }

    public static LwxRecord Lwx_RequireOperationRecord(this HttpContext context)
    {
        return context.GetLwxActivityRecord() ?? throw new InvalidOperationException("OperationRecord is not set in the context.");        
    }

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void SetLwxActivityRecordResponseBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.Lwx_RequireOperationRecord();        
        if (record.ResponseBodyMode != LwxRecordBodyCaptureMode.Ignored)
        {
            throw new InvalidOperationException("The Lwx_AddResponseBodyJson can only be called if the [LwxEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
        }
        else
        {
            record.ResponseBodyJson = JsonSerializer.Serialize(jsonObject);
            record.ResponseBodyType = LwxRecordBodyType.Json;            
            record.ResponseBodyMode = LwxRecordBodyCaptureMode.Custom;
        }        
    }

    /// <summary>
    /// Adds the specified object as the request body JSON.
    /// </summary>
    public static void SetLwxActivityRecordRequestBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.Lwx_RequireOperationRecord();        
        if (record.RequestBodyMode != LwxRecordBodyCaptureMode.Ignored)
        {
            throw new InvalidOperationException("The Lwx_AddRequestBodyJson can only be called if the [LwxEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
        }
        else
        {
            record.RequestBodyJson = JsonSerializer.Serialize(jsonObject);
            record.RequestBodyType = LwxRecordBodyType.Json;            
            record.RequestBodyMode = LwxRecordBodyCaptureMode.Custom;
        }        
    }

    /// <summary>
    /// Adds context information with the specified key and value.
    /// </summary>
    public static void SetLwxActivityRecordContextInfo(this HttpContext context, string key, object value)
    {
        var record = context.Lwx_RequireOperationRecord();

        record.ContextInfo ??= [];

        record.ContextInfo[key] = value;
    }

    /// <summary>
    /// Removes the context information with the specified key.
    /// </summary>
    public static void RemoveLwxActivityRecordContextInfo(this HttpContext context, string key)
    {
        var record = context.Lwx_RequireOperationRecord();

        record.ContextInfo?.Remove(key);
    }

    private static string GenerateConfigErrorMessage(string message)
    {
        return $$"""


            {{message}}

            The LwxActivityLog should be configured in appsettings.json file like:

            "LwxActivityLog": 
            {
                "FixIpAddr": true,
                "IgnoreEndpointsWithoutAttribute": true,
                "ErrorHandler": true
            }

            If you are using kubernetes, and needs to override the config values, you can use the following environment variables:

                LwxActivityLog__FixIpAddr=true
                LwxActivityLog__IgnoreEndpointsWithoutAttribute=true
                LwxActivityLog__ErrorHandler=true

            This is usually in a configmap insice an yaml.

            """;
    }
}

