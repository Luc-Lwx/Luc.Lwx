using System.Text.Json;
using Luc.Lwx.Interface;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Luc.Lwx.LwxActivityLog;

public static class LwxActivityLogContextExtensions
{    
    private const string s_operationRecordKey = "OperationRecord";


    /// <summary>
    /// Sets the specified record in the context.
    /// </summary>
    internal static void SetLwxActivityRecord(this HttpContext context, LwxActivityRecord record)
    {        
        context.Items[s_operationRecordKey] = record;
    }

    /// <summary>
    /// Gets the record from the context.
    /// </summary>
    public static LwxActivityRecord? LwxGetActivityRecord(this HttpContext context)
    {        
        return context.Items.TryGetValue(s_operationRecordKey, out var record) ? record as LwxActivityRecord : null;
    }

    /// <summary>
    /// Gets the record from the context or throws an exception if it is not set.
    /// </summary>
    public static LwxActivityRecord LwxRequireActivityRecord(this HttpContext context)
    {
        return context.LwxGetActivityRecord() ?? throw new InvalidOperationException("OperationRecord is not set in the context.");        
    }

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void LwxSetActivityRecordResponseBodyJson(this HttpContext context, object jsonObject)
    {
        context.LwxSetActivityRecordResponseBody(JsonSerializer.Serialize(jsonObject), LwxRecordBodyType.Json);        
    }

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void LwxSetActivityRecordResponseBody(this HttpContext context, string text, LwxRecordBodyType type)
    {
        var record = context.LwxRequireActivityRecord();    
        record.SetResponseBody(text, type);
    }

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void LwxSetActivityRecordRequestBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.LwxRequireActivityRecord();    
        record.SetResponseBodyJson(jsonObject);
    }
      
    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void LwxSetActivityRecordRequestBody(this HttpContext context, string content, LwxRecordBodyType type )
    {
        var record = context.LwxRequireActivityRecord();        
        record.SetRequestBody(content, type);
    }

    /// <summary>
    /// Adds context information with the specified key and value.
    /// </summary>
    public static void LwxSetActivityRecordContextInfo(this HttpContext context, string key, object value)
    {
        var record = context.LwxRequireActivityRecord();
        record.SetContextInfo(key, value);
    }

    /// <summary>
    /// Removes the context information with the specified key.
    /// </summary>
    public static void LwxRemoveActivityRecordContextInfo(this HttpContext context, string key)
    {
        var record = context.LwxRequireActivityRecord();
        record.RemoveContextInfo(key);
    }
}

