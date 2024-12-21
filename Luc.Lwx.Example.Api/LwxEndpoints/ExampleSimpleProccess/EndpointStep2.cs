using System.Diagnostics.CodeAnalysis;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointStep2
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/step2",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Example Step 2",
      SwaggerFuncSummary = "Executes the second step of the example process",
      SwaggerFuncDescription = "This endpoint handles the execution of the second step in the example process.",
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Executes step 2 of the example process"
    )]
    public async static Task<ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] RequestDto request
    ) 
    {
      // Process the request here
      return new ResponseDto { Ok = true };
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class RequestDto
    {
        [JsonPropertyName("param4")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long Param4 { get; set; }

        [JsonPropertyName("param5")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Param5 { get; set; }

        [JsonPropertyName("param6")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Param6 { get; set; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseDto
    {
        [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required bool Ok { get; set; }
    }
}
