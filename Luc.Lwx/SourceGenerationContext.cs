using System.Text.Json.Serialization;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;

namespace Luc.Lwx;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(LwxResponseDto))]
[JsonSerializable(typeof(LwxActivityRecord))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
