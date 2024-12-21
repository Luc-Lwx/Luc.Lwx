using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.Model;

namespace Luc.Lwx.Example.Api;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(ExampleSimpleProccessCancelRequestDto))]
[JsonSerializable(typeof(ExampleSimpleProccessCancelResponseDto))]
[JsonSerializable(typeof(ExampleSimpleProccessListResponseDto))]
[JsonSerializable(typeof(ExampleSimpleProccessListResponseDto))]
[JsonSerializable(typeof(ExampleSimpleProccessStartRequestDto))]
[JsonSerializable(typeof(ExampleSimpleProccessStartResponseDto))]
[JsonSerializable(typeof(ExampleSimpleProccessStatusResponseDto))]
[JsonSerializable(typeof(ExampleSimpleProccessStep1RequestDto))]
[JsonSerializable(typeof(ExampleSimpleProccessStep1RequestDto))]
[JsonSerializable(typeof(ExampleSimpleProccessStep2RequestDto))]
[JsonSerializable(typeof(ExampleSimpleProccessStep2ResponseDto))]
[JsonSerializable(typeof(ExampleSimpleProccessFinishRequestDto))]
[JsonSerializable(typeof(ExampleSimpleProccessFinishResponseDto))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
