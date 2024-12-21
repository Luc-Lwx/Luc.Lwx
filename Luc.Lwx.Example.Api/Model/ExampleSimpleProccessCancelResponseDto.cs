using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class ExampleSimpleProccessCancelResponseDto
{
    [JsonPropertyName("are_you_sure")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool AreYouSure { get; set; }
}
