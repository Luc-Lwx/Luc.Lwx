using System.Text.Json;

namespace Luc.Lwx.LwxActivityLog;

public static class LwxActivityLogExtensions
{
    public static IApplicationBuilder UseLwxActivityLog(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LwxActivityLogMiddleware>();
    }
    public static WebApplicationBuilder SetLwxActivityLogConfig(this WebApplicationBuilder builder, LwxActivityLogConfig config )
    {        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddSingleton<LwxActivityLogConfig>(config);
        return builder;
    }
    public static WebApplicationBuilder SetLwxActivityLogOutput(this WebApplicationBuilder builder, ILwxActivityLogOutput output )
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
    public static void SetLwxActivityRecordRequestBodyJson(this HttpContext context, object jsonObject)
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
}


