using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Interface;

/// <summary>
/// Base class for all Lwx Framework error responses.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class LwxResponseDto 
{
    [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Ok { get; set; }

    [JsonPropertyName("err-id")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("err-msg")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorMessage { get; set; }
}


