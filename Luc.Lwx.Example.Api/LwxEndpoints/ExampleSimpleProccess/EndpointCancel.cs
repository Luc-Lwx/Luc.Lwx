using System.Diagnostics.CodeAnalysis;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointCancel
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/cancel",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Cancel Example Process",
      SwaggerFuncSummary = "Cancels the example process",
      SwaggerFuncDescription = "This endpoint handles the cancellation of the example process.",
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Cancels the example process"
    )]
    public async static Task<ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] RequestDto request
    ) 
    {
      // Cancel the process here
      return new ResponseDto { Ok = true };
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseDto
    {
        [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Ok { get; set; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class RequestDto
    {
        [JsonPropertyName("are_you_sure")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool AreYouSure { get; set; }
    }
}

