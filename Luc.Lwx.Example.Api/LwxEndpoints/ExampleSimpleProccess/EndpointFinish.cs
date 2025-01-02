
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointFinish
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/finish",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Finish Example Process",
      SwaggerFuncSummary = "Finishes the example process",
      SwaggerFuncDescription = "This endpoint handles the completion of the example process.",
      SwaggerGroupTitle = "Example Proccess (Recomended)"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Finishes the example process"
    )]
    public async static Task<ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] RequestDto request
    ) 
    {
      // Finish the process here
      return new ResponseDto { Ok = true };
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class RequestDto
    {
        [JsonPropertyName("param7")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Param7 { get; set; }

        [JsonPropertyName("param8")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Param8 { get; set; }

        [JsonPropertyName("accepted")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Accepted { get; set; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseDto
    {
        [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required bool Ok { get; set; }
    }
}