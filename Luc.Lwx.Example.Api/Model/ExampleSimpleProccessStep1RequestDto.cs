using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class ExampleSimpleProccessStep1RequestDto
{
    [JsonPropertyName("param1")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Param1 { get; set; }

    [JsonPropertyName("param2")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Param2 { get; set; }

    [JsonPropertyName("param3")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Param3 { get; set; }
}
