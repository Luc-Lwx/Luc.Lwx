using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Example.Api.Model;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointStart
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/start",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Start Example Process",
      SwaggerFuncSummary = "Starts the example process",
      SwaggerFuncDescription = "This endpoint initiates the example process.",
      SwaggerGroupTitle = "Example Proccess (Recomended)"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Starts the example process"
    )]
    public async static Task<ExampleSimpleProccessStartResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromBody] ExampleSimpleProccessStartRequestDto request
    ) 
    {
      // Start the process here
      return new ExampleSimpleProccessStartResponseDto 
      { 
          Ok = true,
          ProcId = 1209830912380192,           
      };
    }

    
}
