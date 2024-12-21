using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class ExampleSimpleProccessListResponseProccessDto
{      
    [JsonPropertyName("id")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]       
    public long Id { get; set; }

    [JsonPropertyName("status")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]       
    public string? Status { get; set; }

    [JsonPropertyName("created-at")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CreatedAt { get; set; }
}
