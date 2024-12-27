namespace Luc.Lwx.LwxActivityLog;

using Luc.Lwx.Util;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Luc.Lwx.Interface;

/// <summary>
/// Represents an operation record for observability purposes.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class LwxActivityRecord
{
    /// <summary>
    /// The timestamp when the operation was recorded.
    /// </summary>
    [JsonPropertyName("dh")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(LwxJsonConverter_DateTimeField))]
    public DateTime When { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The step of the operation (e.g., Start, Step, Submit, Finish).
    /// </summary>
    [JsonPropertyName("step")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(JsonStringEnumConverter<LwxActionStep>))]
    public LwxActionStep? Step { get; set; } = null;

    /// <summary>
    /// The importance level of the operation (e.g., Low, Medium, High, Critical).
    /// </summary>
    [JsonPropertyName("pri")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(JsonStringEnumConverter<LwxActivityImportance>))]
    public LwxActivityImportance? Importance { get; set; } = null;   
    
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
    public LwxRecordBodyType? RequestBodyType { get; set; } = null;

    /// <summary>
    /// The JSON representation of the request body.
    /// </summary>
    [JsonPropertyName("req-body-json")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(LwxJsonConverter_RawField))]
    public string? RequestBodyJson { get; set; } = null;

    [JsonIgnore]
    internal bool RequestBodyCapture { get; set; }

    /// <summary>
    /// Indicates whether the request body is ignored.
    /// </summary>
    [JsonPropertyName("req-body-mode")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonStringEnumConverter<LwxRecordBodyCaptureMode>))]
    public LwxRecordBodyCaptureMode? RequestBodyMode { get; set; } = null;

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
    public LwxRecordBodyType? ResponseBodyType { get; set; }

    /// <summary>
    /// The JSON representation of the response body.
    /// </summary>
    [JsonPropertyName("rsp-body-json")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    [JsonConverter(typeof(LwxJsonConverter_RawField))]
    public string? ResponseBodyJson { get; set; }

    /// <summary>
    /// Indicates whether the response body is ignored.
    /// </summary>
    [JsonPropertyName("rsp-body-mode")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonStringEnumConverter<LwxRecordBodyCaptureMode>))]
    public LwxRecordBodyCaptureMode? ResponseBodyMode { get; set; } = null;

    [JsonIgnore]
    internal bool ResponseBodyCapture { get; set; }

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


  
    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public void SetResponseBodyJson(object jsonObject)
    {
        SetResponseBody(JsonSerializer.Serialize(jsonObject), LwxRecordBodyType.Json);        
    }

    /// <summary>
    /// Adds the specified object as the response body JSON.
    /// </summary>
    public void SetResponseBody(string text, LwxRecordBodyType type)
    {
        if( ResponseBodyCapture ) 
        {
            throw new InvalidOperationException("Response body can only be set if the CaptureResponseBody=false on [LwxActivityLog]");
        }
        ResponseBodyType = type;
        if( type == LwxRecordBodyType.Json)
        {
            ResponseBody = null;
            ResponseBodyJson = text;
        }   
        else
        {
            ResponseBody = text;
            ResponseBodyJson = null;            
        }                                    
        ResponseBodyMode = LwxRecordBodyCaptureMode.Custom;                
    }

    public void SetRequestBody(object jsonObject)
    {
        SetRequestBody(JsonSerializer.Serialize(jsonObject), LwxRecordBodyType.Json);        
    }
      
    public void SetRequestBody(string content, LwxRecordBodyType type )
    {
        RequestBodyType = type;            
        if( type == LwxRecordBodyType.Json)
        {
            RequestBodyJson = content;
            RequestBodyType = type;            
        }   
        else
        {
            RequestBody = content;
            RequestBodyType = type;            
        }            
        RequestBodyMode = LwxRecordBodyCaptureMode.Custom;                
    }

    /// <summary>
    /// Adds context information with the specified key and value.
    /// </summary>
    public void SetContextInfo(string key, object value)
    {
        ContextInfo ??= [];
        ContextInfo[key] = value;
    }

    /// <summary>
    /// Removes the context information with the specified key.
    /// </summary>
    public void RemoveContextInfo(string key)
    {
        ContextInfo?.Remove(key);
    }
}


