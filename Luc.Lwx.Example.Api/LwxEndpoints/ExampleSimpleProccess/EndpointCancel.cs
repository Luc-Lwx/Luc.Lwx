using System.Diagnostics.CodeAnalysis;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.Model;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointCancel
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/cancel",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Cancel Example Process",
      SwaggerFuncSummary = "Cancels the example process",
      SwaggerFuncDescription = "This endpoint handles the cancellation of the example process.",
      SwaggerGroupTitle = "Example Proccess (Recomended)"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Cancels the example process"
    )]
    public async static Task<ExampleSimpleProccessCancelRequestDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] ExampleSimpleProccessCancelResponseDto request
    ) 
    {
      // Cancel the process here
      return new ExampleSimpleProccessCancelRequestDto { Ok = true };
    }

  
}

