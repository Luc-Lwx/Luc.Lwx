namespace Luc.Web.Observability;

using Luc.Web.Interface;
using Luc.Web.Util;
using System.Text.Json.Serialization;


public enum LucWebObservabilityImportance 
{  
  Low,
  Medium,
  High,
  Critical
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

  [JsonPropertyName("req-hdrs")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
  public Dictionary<string,string>? RequestHeaders { get; set; } = null;


  [JsonPropertyName("rsp-hdrs")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
  public Dictionary<string,string>? ResponseHeaders { get; set; }
  
  [JsonPropertyName("rsp-body")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
  public string? ResponseBody { get; set; }

  [JsonPropertyName("req-body-tp")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
  public LucWebBodyType? ResponseBodyType { get; set; }

  [JsonPropertyName("rsp-body-json")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] [JsonConverter(typeof(LucUtilRawJsonConverter))]
  public string? ResponseBodyJson { get; set; }
}

public interface ILucWebObservabilityOutput 
{

    void Publish(OperationRecord record);

}

public class ObservabilityMiddleware 
(
  ILucWebObservabilityOutput output
) : IMiddleware
{



  private static OperationRecord CreateOperationRecord(HttpContext context) 
  {
    var result = new OperationRecord
    {
        RequestPath = context.Request.Path
    };
        
    return result;
  }


  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  { 
      context.Request.EnableBuffering();
      var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
      context.Request.Body.Position = 0;

      var originalBodyStream = context.Response.Body;
      using var responseBodyStream = new MemoryStream();
      context.Response.Body = responseBodyStream;

      var endpoint = context.GetEndpoint();
      var lucEndpointAttribute = endpoint?.Metadata.GetMetadata<LucEndpointAttribute>();

    
      // ...existing code...
      await next(context);
      // ...existing code...

      context.Response.Body.Seek(0, SeekOrigin.Begin);
      var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
      context.Response.Body.Seek(0, SeekOrigin.Begin);

    
      await responseBodyStream.CopyToAsync(originalBodyStream);


      
  }
}
