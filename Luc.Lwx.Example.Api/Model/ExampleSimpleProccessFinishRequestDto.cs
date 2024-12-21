using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ExampleSimpleProccessFinishRequestDto
    {
        [JsonPropertyName("param7")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Param7 { get; set; }

        [JsonPropertyName("param8")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Param8 { get; set; }

        [JsonPropertyName("accepted")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Accepted { get; set; }
    }
