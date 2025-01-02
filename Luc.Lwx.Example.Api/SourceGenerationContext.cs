using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointCancel.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointCancel_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointCancel.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointCancel_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointFinish.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointFinish_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointFinish.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointFinish_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointStart.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointStart_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointStart.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointStart_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointStep1.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointStep1_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointStep1.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointStep1_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointStep2.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointStep2_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointStep2.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointStep2_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointList.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointList_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointList.ResponseProccessDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointList_ResponseProccessDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleSimpleProccess.EndpointStatus.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleSimpleProccess_EndpointStatus_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointCancel.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointCancel_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointCancel.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointCancel_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointFinish.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointFinish_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointFinish.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointFinish_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointStart.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointStart_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointStart.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointStart_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointStep1.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointStep1_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointStep1.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointStep1_ResponseDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointStep2.RequestDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointStep2_RequestDto")]
[JsonSerializable(typeof(LwxEndpoints.ExampleParamInPath.ParamId.EndpointStep2.ResponseDto), TypeInfoPropertyName = "LwxEndpoints_ExampleParamInPath_ParamId_EndpointStep2_ResponseDto")]
public partial class SourceGenerationContext : JsonSerializerContext
{    
}
