namespace Luc.Web.Observability;

using Luc.Web.Interface;
using Luc.Web.Util;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;


public static class LucWebObservabilityExtensions
{
    /// <summary>
    /// Adds the ObservabilityMiddleware to the application's request pipeline.
    /// </summary>
    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ObservabilityMiddleware>();
    }

    /// <summary>
    /// Registers the ObservabilityMiddleware and its dependencies in the service collection.
    /// </summary>
    public static IServiceCollection AddObservabilityConfig(this IServiceCollection services, LucWebObservabilityConfig config )
    {        
        services.AddSingleton<LucWebObservabilityConfig>(config);
        return services;
    }
}

public class LucWebObservabilityConfig
{
    /// <summary>
    /// Indicates whether to ignore endpoints that do not have the [LucEndpoint] attribute.
    /// Default is true.
    /// </summary>
    public bool IgnoreEndpointsWithoutAttribute { get; set; } = true;    
    
    /// <summary>
    /// Indicates whether to fix the IP address and port using the X-Forwarded-For header.
    /// Default is true.
    /// </summary>
    public bool FixIpAddr { get; set; } = true;
    
    /// <summary>
    /// Indicates whether to handle exceptions and generate a standardized error response.
    /// Default is true.
    /// </summary>
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

public enum LucWebCaptureType
{    
    Custom,
    Ignored
}

/// <summary>
/// Represents an operation record for observability purposes.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class OperationRecord
{
    /// <summary>
    /// The timestamp when the operation was recorded.
    /// </summary>
    [JsonPropertyName("dh")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(LucUtilDatetimeConverter))]
    public DateTime When { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The step of the operation (e.g., Start, Step, Submit, Finish).
    /// </summary>
    [JsonPropertyName("step")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebObservabilityStep? Step { get; set; } = null;

    /// <summary>
    /// The importance level of the operation (e.g., Low, Medium, High, Critical).
    /// </summary>
    [JsonPropertyName("pri")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebObservabilityImportance? Importance { get; set; } = null;   
    
    /// <summary>
    /// The host of the request.
    /// </summary>
    [JsonPropertyName("req-host")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Host { get; set; } = null;

    /// <summary>
    /// The remote IP address of the client.
    /// </summary>
    [JsonPropertyName("remote-ip")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RemoteIp { get; set; } = null;

    /// <summary>
    /// The remote port of the client.
    /// </summary>
    [JsonPropertyName("remote-port")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? RemotePort { get; set; } = null;

    /// <summary>
    /// The path of the request.
    /// </summary>
    [JsonPropertyName("req-path")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RequestPath { get; set; } = null;

    /// <summary>
    /// The path parameters of the request.
    /// </summary>
    [JsonPropertyName("req-path-param")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? RequestPathParams { get; set; } = null;
      
    /// <summary>
    /// The query parameters of the request.
    /// </summary>
    [JsonPropertyName("req-query")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? RequestQuery { get; set; } = null;

    /// <summary>
    /// The body of the request.
    /// </summary>
    [JsonPropertyName("req-body")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public string? RequestBody { get; set; } = null;

    /// <summary>
    /// The type of the request body (e.g., Json, Text, Base64).
    /// </summary>
    [JsonPropertyName("req-body-tp")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public LucWebBodyType? RequestBodyType { get; set; } = null;

    /// <summary>
    /// The JSON representation of the request body.
    /// </summary>
    [JsonPropertyName("req-body-json")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(LucUtilRawJsonConverter))]
    public string? RequestBodyJson { get; set; } = null;

    /// <summary>
    /// Indicates whether the request body is ignored.
    /// </summary>
    [JsonPropertyName("req-body-mode")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebCaptureType? RequestBodyMode { get; set; } = null;

    /// <summary>
    /// The headers of the request.
    /// </summary>
    [JsonPropertyName("req-hdrs")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? RequestHeaders { get; set; } = null;

    /// <summary>
    /// The headers of the response.
    /// </summary>
    [JsonPropertyName("rsp-hdrs")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public Dictionary<string,string>? ResponseHeaders { get; set; }

    /// <summary>
    /// The status code of the response.
    /// </summary>
    [JsonPropertyName("rsp-status")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ResponseStatus { get; set; } = null;

    /// <summary>
    /// The body of the response.
    /// </summary>
    [JsonPropertyName("rsp-body")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public string? ResponseBody { get; set; }

    /// <summary>
    /// The type of the response body (e.g., Json, Text, Base64).
    /// </summary>
    [JsonPropertyName("rsp-body-tp")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public LucWebBodyType? ResponseBodyType { get; set; }

    /// <summary>
    /// The JSON representation of the response body.
    /// </summary>
    [JsonPropertyName("rsp-body-json")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(LucUtilRawJsonConverter))]
    public string? ResponseBodyJson { get; set; }

    /// <summary>
    /// Indicates whether the response body is ignored.
    /// </summary>
    [JsonPropertyName("rsp-body-mode")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LucWebCaptureType? ResponseBodyMode { get; set; } = null;

    /// <summary>
    /// Additional context information.
    /// </summary>
    [JsonPropertyName("ctx-info")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string,object>? ContextInfo { get; set; } = null;

    /// <summary>
    /// Subject identifier - unique ID of the user.
    /// Example: "sub": "1234567890"
    /// </summary>
    [JsonPropertyName("auth-user-id")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthUserId { get; set; } = null;
    
    /// <summary>
    /// Preferred username of the user.
    /// Example: "preferred_username": "johndoe"
    /// </summary>
    [JsonPropertyName("auth-user-name")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthUserName { get; set; } = null;
    
    /// <summary>
    /// Email address of the user.
    /// Example: "email": "johndoe@example.com"
    /// </summary>
    [JsonPropertyName("auth-user-email")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthEmail { get; set; } = null;
    
    /// <summary>
    /// Full name of the user.
    /// Example: "name": "John Doe"
    /// </summary>
    [JsonPropertyName("auth-user-fullname")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthFullName { get; set; } = null;    
    
    /// <summary>
    /// Authorized party - client ID of the application.
    /// Example: "azp": "myclient"
    /// </summary>
    [JsonPropertyName("auth-azp")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthAuthorizedParty { get; set; } = null;
    
    /// <summary>
    /// Audience - intended recipients of the token.
    /// Example: "aud": "myclient"
    /// </summary>
    [JsonPropertyName("auth-aud")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthAudience { get; set; } = null;
    
    /// <summary>
    /// Roles assigned to the user in the realm.
    /// Example: "realm_access": "{\"roles\":[\"user\",\"admin\"]}"
    /// </summary>
    [JsonPropertyName("auth-roles")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthRoles { get; set; } = null;
    
    /// <summary>
    /// Roles assigned to the user for specific resources.
    /// Example: "resource_access": "{\"myclient\":{\"roles\":[\"custom-role\"]}}"
    /// </summary>
    [JsonPropertyName("auth-resources")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthResources { get; set; } = null;

    /// <summary>
    /// Additional claims not covered by the standard properties.
    /// </summary>
    [JsonPropertyName("auth-extra")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, string>? AuthExtra { get; set; } = null;
}

/// <summary>
/// Represents the output interface for observability.
/// </summary>
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

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public static void LucWebAddResponseBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.LucWebRequireOperationRecord();        
        if( record.ResponseBodyMode != LucWebCaptureType.Ignored )
        {
            throw new InvalidOperationException("The LucWebAddResponseBody can only be called if the [LucEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
        }
        else
        {
            record.ResponseBodyJson = JsonSerializer.Serialize(jsonObject);
            record.ResponseBodyType = LucWebBodyType.Json;            
            record.ResponseBodyMode = LucWebCaptureType.Custom;
        }        
    }

    /// <summary>
    /// Adds the specified object as the request body JSON.
    /// </summary>
    public static void LucWebAddRequestBodyJson(this HttpContext context, object jsonObject)
    {
        var record = context.LucWebRequireOperationRecord();        
        if( record.RequestBodyMode != LucWebCaptureType.Ignored )
        {
            throw new InvalidOperationException("The LucWebAddResponseBody can only be called if the [LucEndpoint] attribute does have the ObservabilityIgnoreResponseBody=true.");
        }
        else
        {
            record.RequestBodyJson = JsonSerializer.Serialize(jsonObject);
            record.RequestBodyType = LucWebBodyType.Json;            
            record.RequestBodyMode = LucWebCaptureType.Custom;
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

public class ObservabilityMiddleware 
(
    ILucWebObservabilityOutput output,
    LucWebObservabilityConfig config
) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    { 
        if( config.FixIpAddr )
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
        if( record.ResponseBodyMode != LucWebCaptureType.Ignored )
        {
            originalBodyStream = context.Response.Body;
            context.Response.Body = new MemoryStream();
        }
        else
        {
            originalBodyStream = Stream.Null;
        }

        UpdateRequestHeaders(record, context);
      
        if( record.RequestBodyMode != LucWebCaptureType.Ignored )
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
            if (config.ErrorHandler)
            {
                await HandleGeneralException(e, record, context);
            }
            else
            {
                throw;
            }
        }

        UpdateResponseHeaders(record, context);

        if( record.ResponseBodyMode != LucWebCaptureType.Ignored )
        {
            await UpdateResponseBody(record, context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
        }

        output.Publish(record);
    }
   
    /// <summary>
    /// Creates an OperationRecord based on the current HttpContext.
    /// </summary>
    private OperationRecord? CreateOperationRecord(HttpContext context) 
    {
        var endpoint = context.GetEndpoint();
        var lucEndpointAttribute = endpoint?.Metadata.GetMetadata<LucEndpointAttribute>();
        if (lucEndpointAttribute == null) 
        {            
            if( config.IgnoreEndpointsWithoutAttribute )
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
            RequestBodyMode = lucEndpointAttribute?.ObservabilityRequestBodyMode,
            ResponseBodyMode = lucEndpointAttribute?.ObservabilityResponseBodyMode
        };        

        // when the user declared that will provide a custom body, we need to set the default to ignored         
        if( result.RequestBodyMode == LucWebCaptureType.Custom )
        {
            result.RequestBodyMode = LucWebCaptureType.Ignored;
        }
        // when the user declared that will provide a custom body, we need to set the default to ignored 
        if( result.ResponseBodyMode == LucWebCaptureType.Custom )
        {
            result.ResponseBodyMode = LucWebCaptureType.Ignored;
        }        
        return result;
    }

    /// <summary>
    /// Extracts and parses the JWT token information from the HttpContext.
    /// </summary>
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
                    case "given_name":
                        // Ignore given name since full name is captured
                        break;
                    case "family_name":
                        // Ignore family name since full name is captured
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

    /// <summary>
    /// Updates the IP address and port of the request using the X-Forwarded-For header.
    /// </summary>
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

    /// <summary>
    /// Updates the request headers of the operation record.
    /// </summary>
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

    /// <summary>
    /// Updates the response headers of the operation record.
    /// </summary>
    private static void UpdateResponseHeaders(OperationRecord record, HttpContext context)
    {
        record.ResponseHeaders = context.Response.Headers
            .GroupBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => string.Join("\r\n", g.SelectMany(v => v.Value))
            );
    }

    /// <summary>
    /// Updates the request body of the operation record.
    /// </summary>
    private static async Task UpdateRequestBody(OperationRecord record, HttpContext context)
    {
        if (record.RequestBodyMode != LucWebCaptureType.Ignored)
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
   
    /// <summary>
    /// Updates the response body of the operation record.
    /// </summary>
    private static async Task UpdateResponseBody(OperationRecord record, HttpContext context)
    {
        if (record.ResponseBodyMode != LucWebCaptureType.Ignored)
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

    /// <summary>
    /// Handles a LucWebResponseException by generating a standardized error response.
    /// </summary>
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

    /// <summary>
    /// Handles a general exception by generating a standardized error response.
    /// </summary>
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


