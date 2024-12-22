using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Example.Api.Model;
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
      SwaggerGroupTitle = "Example Proccess (Recomended)"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Retrieves the status of the example process"
    )]
    public async static Task<ExampleSimpleProccessStatusResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id
    ) 
    {
      // Retrieve the process status here
      return new ExampleSimpleProccessStatusResponseDto 
      { 
          Ok = true,
          Status = "active"
      };
    }

    
}
