namespace Luc.Web.Observability;

using Luc.Web.Interface;
using Luc.Web.Util;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

public class LucWebObservabilityConfig
{
    public bool IgnoreEndpointsWithoutAttribute { get; set; } = true;    
    public bool FixIpAddr { get; set; } = true;
    public bool ErrorHandler { get; set; } = true;
}

public enum LucWebObservabilityImportance 
{  
    Ignore,
    Low,
    Medium,
    High,
    Critical,    
}
public enum LucWebObservabilityStep
{  
    Start,
    Step,
    Submit,
    Finish
}

public enum LucWebBodyType 
{
    Json,
    Text,
    Base64
}


public class OperationRecord
{    
    [JsonPropertyName("dh")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(LucUtilDatetimeConverter))]
    public DateTime When { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("step")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebObservabilityStep? Step { get; set; } = null;

    [JsonPropertyName("pri")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebObservabilityImportance? Importance { get; set; } = null;   
    
    [JsonPropertyName("req-path")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RequestPath { get; set; } = null;

    [JsonPropertyName("req-path-param")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? RequestPathParams { get; set; } = null;
      
    [JsonPropertyName("req-query")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? RequestQuery { get; set; } = null;

    [JsonPropertyName("req-body")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public string? RequestBody { get; set; } = null;

    [JsonPropertyName("req-body-tp")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public LucWebBodyType? RequestBodyType { get; set; } = null;

    [JsonPropertyName("req-body-json")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(LucUtilRawJsonConverter))]
    public string? RequestBodyJson { get; set; } = null;

    [JsonPropertyName("req-body-ignored")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RequestBodyIgnored { get; set; } = null;

    [JsonPropertyName("req-hdrs")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? RequestHeaders { get; set; } = null;

    [JsonPropertyName("rsp-hdrs")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? ResponseHeaders { get; set; }

    [JsonPropertyName("rsp-status")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ResponseStatus { get; set; } = null;


    [JsonPropertyName("rsp-body")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public string? ResponseBody { get; set; }

    [JsonPropertyName("rsp-body-tp")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public LucWebBodyType? ResponseBodyType { get; set; }

    [JsonPropertyName("rsp-body-json")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(LucUtilRawJsonConverter))]
    public string? ResponseBodyJson { get; set; }

    [JsonPropertyName("rsp-body-ignored")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ResponseBodyIgnored { get; set; } = null;
}

public interface ILucWebObservabilityOutput 
{
    void Publish(OperationRecord record);
}

public class ObservabilityMiddleware 
(
    ILucWebObservabilityOutput output,
    IOptions<LucWebObservabilityConfig> config
) : IMiddleware
{
    private OperationRecord? CreateOperationRecord(HttpContext context) 
    {
        var endpoint = context.GetEndpoint();
        var lucEndpointAttribute = endpoint?.Metadata.GetMetadata<LucEndpointAttribute>();
        if (lucEndpointAttribute == null) 
        {            
            if( config.Value.IgnoreEndpointsWithoutAttribute )
            {
                return null;
            }             
        }
        else if( lucEndpointAttribute.ObservabilityImportance == LucWebObservabilityImportance.Ignore) 
        {
            return null;
        }
        
        var routePattern = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.RouteEndpoint>()?.RoutePattern.RawText;
        var routeValues = context.Request.RouteValues;
        var requestPathParams = routeValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? string.Empty);

        if (routePattern == null)
        {
            routePattern = "{path}";
            requestPathParams["path"] = context.Request.Path.ToString();
        }

        var result = new OperationRecord
        {
            RequestPath = routePattern, 
            RequestPathParams = requestPathParams,
            Importance = lucEndpointAttribute?.ObservabilityImportance,
            Step = lucEndpointAttribute?.ObservabilityStep,   
            RequestBodyIgnored = lucEndpointAttribute?.ObservabilityIgnoreRequestBody,         
            ResponseBodyIgnored = lucEndpointAttribute?.ObservabilityIgnoreResponseBody,         
        };        
        return result;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    { 
        if( config.Value.FixIpAddr )
        {
            UpdateRequestIpAndPort(context); 
        }
        
        var record = CreateOperationRecord(context);
        if( record == null )
        {
            await next(context);
            return;
        }

        Stream originalBodyStream;
        if( record.ResponseBodyIgnored != true )
        {
            originalBodyStream = context.Response.Body;
            context.Response.Body = new MemoryStream();
        }
        else
        {
            originalBodyStream = Stream.Null;
        }

        try
        {
            await next(context);
        }
        catch( LucWebResponseException e )
        {   
            var response = new LucWebResponseBase
            {
                Ok = false,
                ErrorCode = e.StatusCode.ToString(),
                ErrorMessage = e.Message
            };

            record.ResponseStatus = e.StatusCode;
            record.ResponseBodyType = LucWebBodyType.Json;
            record.ResponseBodyJson = JsonSerializer.Serialize(response);
            
            context.Response.StatusCode = e.StatusCode;
            context.Response.ContentType = "application/json";                
            await context.Response.WriteAsync(record.ResponseBody ?? "");                
        }
        catch( Exception e )
        {
            if( config.Value.ErrorHandler )
            {
                var response = new LucWebResponseBase
                {
                    Ok = false,
                    ErrorCode = "500",
                    ErrorMessage = e.Message
                };

                record.ResponseBodyType = LucWebBodyType.Json;
                record.ResponseBodyJson = JsonSerializer.Serialize(response);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";                
                await context.Response.WriteAsync(record.ResponseBody ?? "");                
            }
            else 
            {
                throw;
            }
        }

        UpdateRequestHeaders(record, context);
        UpdateResponseHeaders(record, context);

        if( record.RequestBodyIgnored != true )
        {
            await UpdateRequestBody(record, context);
        }

        if( record.ResponseBodyIgnored != true )
        {
            await UpdateResponseBody(record, context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
        }
    }

    private static void UpdateRequestIpAndPort(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ipPort = forwardedFor.FirstOrDefault()?.Split(':');
            if (ipPort != null && ipPort.Length > 0)
            {
                context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ipPort[0]);
                if (ipPort.Length > 1 && int.TryParse(ipPort[1], out var port))
                {
                    context.Connection.RemotePort = port;
                }
                else 
                {
                    context.Connection.RemotePort = 0;
                }
            }
        }
    }

    private static void UpdateRequestHeaders(OperationRecord record, HttpContext context)
    {
        record.RequestHeaders = context.Request.Headers
            .GroupBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => string.Join("\r\n", g.SelectMany(v => v.Value))
            );
    }

    private static void UpdateResponseHeaders(OperationRecord record, HttpContext context)
    {
        record.ResponseHeaders = context.Response.Headers
            .GroupBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => string.Join("\r\n", g.SelectMany(v => v.Value))
            );
    }

    private static async Task UpdateRequestBody(OperationRecord record, HttpContext context)
    {
        if( record.RequestBodyIgnored != true )
        {
            context.Request.EnableBuffering();
            record.RequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
        }
    }

    private static async Task UpdateResponseBody(OperationRecord record, HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        record.ResponseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
    }
}
