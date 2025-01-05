using System.Text.Json.Serialization;

namespace Luc.Lwx.Interface;

/// <summary>
/// Interface for all Lwx Framework responses that are considered successful.
/// </summary>
public interface ILwxResponseErr
{
    /// <summary>
    /// Gets or sets a value indicating whether the response is successful.
    /// </summary>
    [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    bool? Ok { get; set; }

    /// <summary>   
    /// Gets or sets the error code.
    /// </summary>    
    [JsonPropertyName("err-id")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [JsonPropertyName("err-msg")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorMessage { get; set; }
}