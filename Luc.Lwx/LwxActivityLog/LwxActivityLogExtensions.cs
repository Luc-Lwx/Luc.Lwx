using System.Text.Json;

namespace Luc.Lwx.LwxActivityLog;

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

    internal static void Lwx_SetOperationRecord(this HttpContext context, LwxRecord record)
    {        
        context.Items[s_operationRecordKey] = record;
    }

    public static LwxRecord? Lwx_GetOperationRecord(this HttpContext context)
    {        
        return context.Items.TryGetValue(s_operationRecordKey, out var record) ? record as LwxRecord : null;
    }

    public static LwxRecord Lwx_RequireOperationRecord(this HttpContext context)
    {
        return context.Lwx_GetOperationRecord() ?? throw new InvalidOperationException("OperationRecord is not set in the context.");        
    }

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void Lwx_AddResponseBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.Lwx_RequireOperationRecord();        
        if( record.ResponseBodyMode != LwxRecordBodyCaptureMode.Ignored )
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
    public static void Lwx_AddRequestBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.Lwx_RequireOperationRecord();        
        if( record.RequestBodyMode != LwxRecordBodyCaptureMode.Ignored )
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
    public static void Lwx_AddContextInfo(this HttpContext context, string key, object value)
    {
        var record = context.Lwx_RequireOperationRecord();

        record.ContextInfo ??= [];

        record.ContextInfo[key] = value;
    }

    /// <summary>
    /// Removes the context information with the specified key.
    /// </summary>
    public static void Lwx_RemoveContextInfo(this HttpContext context, string key)
    {
        var record = context.Lwx_RequireOperationRecord();

        record.ContextInfo?.Remove(key);
    }
}


