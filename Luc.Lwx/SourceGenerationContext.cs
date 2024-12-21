using System.Text.Json.Serialization;
using Luc.Lwx.Interface;

namespace Luc.Lwx;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(LwxResponseDto))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
