using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class ExampleParamInPathStep2ResponseDto
{
    [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Ok { get; set; }
}
