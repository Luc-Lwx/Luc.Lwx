using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointStatus
{ 
    [LwxEndpoint(
      Path = "GET /apimanager-prefix/example-simple-proccess/status",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Example Process Status",
      SwaggerFuncSummary = "Gets the status of the example process",
      SwaggerFuncDescription = "This endpoint retrieves the current status of the example process.",
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Retrieves the status of the example process"
    )]
    public async static Task<ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id
    ) 
    {
      // Retrieve the process status here
      return new ResponseDto 
      { 
          Ok = true,
          Status = "active"
      };
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseDto
    {
        [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required bool Ok { get; set; }

        [JsonPropertyName("status")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Status { get; set; }
    }
}
