
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Example.Api.Model;
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
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Finishes the example process"
    )]
    public async static Task<ExampleSimpleProccessFinishResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] ExampleSimpleProccessFinishRequestDto request
    ) 
    {
      // Finish the process here
      return new ExampleSimpleProccessFinishResponseDto { Ok = true };
    }

    
}