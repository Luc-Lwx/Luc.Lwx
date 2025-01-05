using System.Text.Json.Serialization;

namespace Luc.Lwx.Interface;

/// <summary>
/// Interface for all Lwx Framework responses that are considered successful.
/// </summary>
public interface ILwxResponseOk
{
    /// <summary>
    /// Gets or sets a value indicating whether the response is successful.
    /// </summary>
    [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    bool? Ok { get; set; }
}