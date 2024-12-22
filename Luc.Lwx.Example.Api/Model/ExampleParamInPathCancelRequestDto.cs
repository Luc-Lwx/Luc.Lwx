using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class ExampleParamInPathCancelRequestDto
{
    [JsonPropertyName("data")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Data { get; set; }
}