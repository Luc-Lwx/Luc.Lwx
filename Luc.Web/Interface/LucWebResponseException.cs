using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Web.Interface;

namespace Luc.Web.Interface;

/// <summary>
/// Base class for all Luc.Web error responses.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class LucWebResponseBase 
{
    [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Ok { get; set; }

    [JsonPropertyName("err-id")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("err-msg")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorMessage { get; set; }
}

public class LucWebResponseException
(
    int statusCode,
    string errorMessage,
    object? errorDetails = null        
) : Exception(errorMessage)
{
    public int StatusCode { get; } = statusCode;
    public object? ErrorDetails { get; } = errorDetails;
}



