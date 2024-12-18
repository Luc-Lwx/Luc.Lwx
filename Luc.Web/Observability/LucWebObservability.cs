namespace Luc.Web.Observability;

using Luc.Web.Interface;
using Luc.Web.Util;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

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


[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class OperationRecord
{    
    [JsonPropertyName("dh")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(LucUtilDatetimeConverter))]
    public DateTime When { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("step")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebObservabilityStep? Step { get; set; } = null;

    [JsonPropertyName("pri")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebObservabilityImportance? Importance { get; set; } = null;   
    
    [JsonPropertyName("req-host")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Host { get; set; } = null;

    [JsonPropertyName("remote-ip")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RemoteIp { get; set; } = null;

    [JsonPropertyName("remote-port")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? RemotePort { get; set; } = null;

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

    [JsonPropertyName("ctx-info")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string,object>? ContextInfo { get; set; } = null;

    [JsonPropertyName("auth-user-id")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Subject identifier - unique ID of the user
    // Example: "sub": "1234567890"
    public string? AuthUserId { get; set; } = null;
    
    [JsonPropertyName("auth-user-name")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Preferred username of the user
    // Example: "preferred_username": "johndoe"
    public string? AuthUserName { get; set; } = null;
    
    [JsonPropertyName("auth-user-email")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Email address of the user
    // Example: "email": "johndoe@example.com"
    public string? AuthEmail { get; set; } = null;
    
    [JsonPropertyName("auth-user-fullname")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Full name of the user
    // Example: "name": "John Doe"
    public string? AuthFullName { get; set; } = null;    
    
    [JsonPropertyName("auth-azp")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Authorized party - client ID of the application
    // Example: "azp": "myclient"
    public string? AuthAuthorizedParty { get; set; } = null;
    
    [JsonPropertyName("auth-aud")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Audience - intended recipients of the token
    // Example: "aud": "myclient"
    public string? AuthAudience { get; set; } = null;
    
    [JsonPropertyName("auth-roles")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Roles assigned to the user in the realm
    // Example: "realm_access": "{\"roles\":[\"user\",\"admin\"]}"
    public string? AuthRoles { get; set; } = null;
    
    [JsonPropertyName("auth-resources")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    // Roles assigned to the user for specific resources
    // Example: "resource_access": "{\"myclient\":{\"roles\":[\"custom-role\"]}}"
    public string? AuthResources { get; set; } = null;

    [JsonPropertyName("auth-extra")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, string>? AuthExtra { get; set; } = null
}

public interface ILucWebObservabilityOutput 
{
    void Publish(OperationRecord record);
}

public static class HttpContextExtensions
{
    private const string s_operationRecordKey = "OperationRecord";

    internal static void LucWebSetOperationRecord(this HttpContext context, OperationRecord record)
    {        
        context.Items[s_operationRecordKey] = record;
    }

    public static OperationRecord? LucWebGetOperationRecord(this HttpContext context)
    {        
        return context.Items.TryGetValue(s_operationRecordKey, out var record) ? record as OperationRecord : null;
    }

    public static OperationRecord LucWebRequireOperationRecord(this HttpContext context)
    {
        return context.LucWebGetOperationRecord() ?? throw new InvalidOperationException("OperationRecord is not set in the context.");        
    }

    public static void LucWebAddResponseBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.LucWebRequireOperationRecord();        
        if( record.ResponseBodyIgnored == true )
        {
            throw new InvalidOperationException("The LucWebAddResponseBody can only be called if the [LucEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
        }
        else
        {
            record.ResponseBodyJson = JsonSerializer.Serialize(jsonObject);
            record.ResponseBodyType = LucWebBodyType.Json;            
        }        
    }

    public static void LucWebAddRequestBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.LucWebRequireOperationRecord();        
        if( record.RequestBodyIgnored == true )
        {
            throw new InvalidOperationException("The LucWebAddResponseBody can only be called if the [LucEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
        }
        else
        {
            record.RequestBodyJson = JsonSerializer.Serialize(jsonObject);
            record.RequestBodyType = LucWebBodyType.Json;            
        }        
    }

    public static void LucWebAddContextInfo(this HttpContext context, string key, object value)
    {
        var record = context.LucWebRequireOperationRecord();

        if (record.ContextInfo == null)
        {
            record.ContextInfo = [];
        }

        record.ContextInfo[key] = value;
    }

    public static void LucWebRemoveContextInfo(this HttpContext context, string key)
    {
        var record = context.LucWebRequireOperationRecord();

        record.ContextInfo?.Remove(key);
    }
}

public class ObservabilityMiddleware 
(
    ILucWebObservabilityOutput output,
    IOptions<LucWebObservabilityConfig> config
) : IMiddleware
{
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

        context.LucWebSetOperationRecord(record);

        // Set Host, RemoteIp, and RemotePort
        record.Host = context.Request.Host.Value;
        record.RemoteIp = context.Connection.RemoteIpAddress?.ToString();
        record.RemotePort = context.Connection.RemotePort;

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

        UpdateRequestHeaders(record, context);
      
        if( record.RequestBodyIgnored != true )
        {
            await UpdateRequestBody(record, context);
        }

        // Extract and parse JWT token
        ExtractJwtTokenInfo(record, context);

        try
        {
            await next(context);
        }
        catch (LucWebResponseException e)
        {
            await HandleLucWebResponseException(e, record, context);
        }
        catch (Exception e)
        {
            if (config.Value.ErrorHandler)
            {
                await HandleGeneralException(e, record, context);
            }
            else
            {
                throw;
            }
        }

        UpdateResponseHeaders(record, context);

        if( record.ResponseBodyIgnored != true )
        {
            await UpdateResponseBody(record, context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
        }

        output.Publish(record);
    }

    private static void ExtractJwtTokenInfo(OperationRecord record, HttpContext context)
    {
        var user = context.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            foreach (var claim in user.Claims)
            {
                switch (claim.Type)
                {
                    case "sub":
                        // Subject identifier - unique ID of the user
                        // Example: "sub": "1234567890"
                        record.AuthUserId = claim.Value;
                        break;
                    case "preferred_username":
                        // Preferred username of the user
                        // Example: "preferred_username": "johndoe"
                        record.AuthUserName = claim.Value;
                        break;
                    case "email":
                        // Email address of the user
                        // Example: "email": "johndoe@example.com"
                        record.AuthEmail = claim.Value;
                        break;
                    case "name":
                        // Full name of the user
                        // Example: "name": "John Doe"
                        record.AuthFullName = claim.Value;
                        break;
                    case "azp":
                        // Authorized party - client ID of the application
                        // Example: "azp": "myclient"
                        record.AuthAuthorizedParty = claim.Value;
                        break;
                    case "aud":
                        // Audience - intended recipients of the token
                        // Example: "aud": "myclient"
                        record.AuthAudience = claim.Value;
                        break;
                    case "realm_access":
                        // Roles assigned to the user in the realm
                        // Example: "realm_access": "{\"roles\":[\"user\",\"admin\"]}"
                        record.AuthRoles = claim.Value;
                        break;
                    case "resource_access":
                        // Roles assigned to the user for specific resources
                        // Example: "resource_access": "{\"myclient\":{\"roles\":[\"custom-role\"]}}"
                        record.AuthResources = claim.Value;
                        break;
                    // ...existing code...
                    default:
                         // Capture extra claims
                        record.AuthExtra ??= [];
                        record.AuthExtra[claim.Type] = claim.Value;
                        break;
                }
            }
        }
    }

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

    private static void UpdateRequestIpAndPort(HttpContext context)
    {
        // Examples generated by Copilot
        // X-Forwarded-For: 203.0.113.195
        // X-Forwarded-For: 203.0.113.195, 70.41.3.18, 150.172.238.178
        // X-Forwarded-For: 2001:0db8:85a3:0000:0000:8a2e:0370:7334
        // X-Forwarded-For: 2001:0db8:85a3:0000:0000:8a2e:0370:7334, 203.0.113.195
        // X-Forwarded-For: 203.0.113.195:8080
        // X-Forwarded-For: [2001:0db8:85a3:0000:0000:8a2e:0370:7334]:8080

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ipPort = forwardedFor.FirstOrDefault()?.Split(',');
            if (ipPort != null && ipPort.Length > 0)
            {
                var ip = ipPort[0].Trim();
                
                // Save the original IP and port for logging purposes
                context.Request.Headers["Original-Remote-Ip"] = ip;

                // Try parsing the IP address without port first
                if (System.Net.IPAddress.TryParse(ip, out var ipAddress))
                {
                    context.Connection.RemoteIpAddress = ipAddress;
                    context.Connection.RemotePort = 0;
                }
                else if (System.Net.IPEndPoint.TryParse(ip, out var endPoint))
                {
                    context.Connection.RemoteIpAddress = endPoint.Address;
                    context.Connection.RemotePort = endPoint.Port;
                }
            }
        }
    }

    private static void UpdateRequestHeaders(OperationRecord record, HttpContext context)
    {
        record.RequestHeaders = context.Request.Headers
            .Where(kvp => !string.Equals(kvp.Key, "Host", StringComparison.OrdinalIgnoreCase))
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
        if (record.RequestBodyIgnored != true)
        {
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            var contentType = context.Request.ContentType?.ToLower(CultureInfo.InvariantCulture) ?? throw new InvalidOperationException("Request content type is null.");
            var mediaType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
            var charset = mediaType.CharSet?.ToLower(CultureInfo.InvariantCulture);

            if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Application.Json && charset == "utf-8")
            {
                record.RequestBodyType = LucWebBodyType.Json;
                record.RequestBodyJson = requestBody;
            }
            else if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Text.Plain)
            {
                record.RequestBody = requestBody;
                record.RequestBodyType = LucWebBodyType.Text;
            }
            else
            {
                record.RequestBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestBody));
                record.RequestBodyType = LucWebBodyType.Base64;
            }
        }
    }
   
    private static async Task UpdateResponseBody(OperationRecord record, HttpContext context)
    {
        if (record.ResponseBodyIgnored != true)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var contentType = (context.Response.ContentType?.ToLower(CultureInfo.InvariantCulture)) ?? throw new InvalidOperationException("Response content type is null.");
            var mediaType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
            var charset = mediaType.CharSet?.ToLower(CultureInfo.InvariantCulture);

            if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Application.Json && charset == "utf-8")
            {
                record.ResponseBodyJson = responseBody;
                record.ResponseBodyType = LucWebBodyType.Json;
            }
            else if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Text.Plain)
            {
                record.ResponseBody = responseBody;
                record.ResponseBodyType = LucWebBodyType.Text;
            }
            else
            {
                record.ResponseBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseBody));
                record.ResponseBodyType = LucWebBodyType.Base64;
            }
        }
    }

    private static async Task HandleLucWebResponseException(LucWebResponseException e, OperationRecord record, HttpContext context)
    {
        var response = new LucWebResponseBase
        {
            Ok = false,
            ErrorCode = e.StatusCode.ToString(),
            ErrorMessage = e.Message
        };
        var responseString = JsonSerializer.Serialize(response);

        record.ResponseStatus = e.StatusCode;
        record.ResponseBodyType = LucWebBodyType.Json;
        record.ResponseBodyJson = responseString;

        context.Response.StatusCode = e.StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(responseString);
    }

    private static async Task HandleGeneralException(Exception e, OperationRecord record, HttpContext context)
    {
        var response = new LucWebResponseBase
        {
            Ok = false,
            ErrorCode = "500",
            ErrorMessage = e.Message
        };
        var responseString = JsonSerializer.Serialize(response);

        record.ResponseBodyType = LucWebBodyType.Json;
        record.ResponseBodyJson = responseString;
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(responseString);
    }
}
