using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;



public static partial class EndpointStep1
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/step1",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Example Step 1",
      SwaggerFuncSummary = "Executes the first step of the example process",
      SwaggerFuncDescription = "This endpoint handles the execution of the first step in the example process.",
      SwaggerGroupTitle = "Example Proccess (Recomended)"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Executes step 1 of the example process"
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
        [JsonPropertyName("param1")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Param1 { get; set; }

        [JsonPropertyName("param2")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Param2 { get; set; }

        [JsonPropertyName("param3")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Param3 { get; set; }
    }


    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseDto
    {
        [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required bool Ok { get; set; }
    }

}
