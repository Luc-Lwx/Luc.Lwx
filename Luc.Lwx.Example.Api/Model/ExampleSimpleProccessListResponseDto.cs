using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class ExampleSimpleProccessListResponseDto
{
    [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]                 
    public required bool Ok { get; set; }
    
    [JsonPropertyName("proc-list")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]                 
    public ExampleSimpleProccessListResponseProccessDto[]? ProcList { get; set; }
}
