using System.Diagnostics.CodeAnalysis;
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
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Retrieves the status of the example process"
    )]
    public async static Task<StatusResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id
    ) 
    {
      // Retrieve the process status here
      return new StatusResponseDto 
      { 
          ProcId = proc_id, 
          Status = "active", 
          StepsCompleted = 1, 
          CreatedAt = "2023-10-01T12:00:00Z", 
          LastUpdated = "2023-10-01T12:30:00Z" 
      };
    }

    public class StatusResponseDto
    {
        public decimal ProcId { get; set; }
        public string Status { get; set; }
        public int StepsCompleted { get; set; }
        public string CreatedAt { get; set; }
        public string LastUpdated { get; set; }
    }
}
