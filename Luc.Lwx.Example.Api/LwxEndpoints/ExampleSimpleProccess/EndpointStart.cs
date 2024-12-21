using System.Diagnostics.CodeAnalysis;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
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
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Starts the example process"
    )]
    public async static Task<StartResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromBody] StartRequestDto request
    ) 
    {
      // Start the process here
      return new StartResponseDto 
      { 
          ProcId = 123, 
          Status = "started", 
          StartedAt = "2023-10-01T12:00:00Z" 
      };
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class StartRequestDto
    {
        public int Abc { get; set; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class StartResponseDto
    {
        public int ProcId { get; set; }
        public string Status { get; set; }
        public string StartedAt { get; set; }
    }
}
