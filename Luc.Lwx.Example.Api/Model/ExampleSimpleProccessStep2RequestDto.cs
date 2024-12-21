using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class ExampleSimpleProccessStep2RequestDto
{
    [JsonPropertyName("param4")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long Param4 { get; set; }

    [JsonPropertyName("param5")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Param5 { get; set; }

    [JsonPropertyName("param6")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Param6 { get; set; }
}
