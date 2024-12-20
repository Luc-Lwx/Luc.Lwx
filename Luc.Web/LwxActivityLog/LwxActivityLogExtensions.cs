using System.Text.Json;

namespace Luc.Web.LwxActivityLog;

public static class LwxActivityLogExtensions
{
    /// <summary>
    /// Adds the ObservabilityMiddleware to the application's request pipeline.
    /// </summary>
    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LwxActivityLogMiddleware>();
    }

    /// <summary>
    /// Registers the ObservabilityMiddleware and its dependencies in the service collection.
    /// </summary>
    public static IServiceCollection AddObservabilityConfig(this IServiceCollection services, LwxActivityLogConfig config )
    {        
        services.AddSingleton<LwxActivityLogConfig>(config);
        return services;
    }

    private const string s_operationRecordKey = "OperationRecord";

    internal static void LucWebSetOperationRecord(this HttpContext context, LwxRecord record)
    {        
        context.Items[s_operationRecordKey] = record;
    }

    public static LwxRecord? LucWebGetOperationRecord(this HttpContext context)
    {        
        return context.Items.TryGetValue(s_operationRecordKey, out var record) ? record as LwxRecord : null;
    }

    public static LwxRecord LucWebRequireOperationRecord(this HttpContext context)
    {
        return context.LucWebGetOperationRecord() ?? throw new InvalidOperationException("OperationRecord is not set in the context.");        
    }

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void LucWebAddResponseBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.LucWebRequireOperationRecord();        
        if( record.ResponseBodyMode != LwxRecordBodyCaptureMode.Ignored )
        {
            throw new InvalidOperationException("The LucWebAddResponseBody can only be called if the [LwxEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
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
    public static void LucWebAddRequestBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.LucWebRequireOperationRecord();        
        if( record.RequestBodyMode != LwxRecordBodyCaptureMode.Ignored )
        {
            throw new InvalidOperationException("The LucWebAddResponseBody can only be called if the [LwxEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
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
    public static void LucWebAddContextInfo(this HttpContext context, string key, object value)
    {
        var record = context.LucWebRequireOperationRecord();

        record.ContextInfo ??= [];

        record.ContextInfo[key] = value;
    }

    /// <summary>
    /// Removes the context information with the specified key.
    /// </summary>
    public static void LucWebRemoveContextInfo(this HttpContext context, string key)
    {
        var record = context.LucWebRequireOperationRecord();

        record.ContextInfo?.Remove(key);
    }
}


