using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointList
{ 
    [LwxEndpoint(
      Path = "GET /apimanager-prefix/example-simple-proccess/list",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "List Example Processes",
      SwaggerFuncSummary = "Lists all example processes",
      SwaggerFuncDescription = "This endpoint retrieves a list of all example processes based on the provided filters.",
      SwaggerGroupTitle = "Example Proccess (Recomended)"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Lists all example processes"
    )]
    public async static Task<ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="filter1")] string? filter1,
      [FromQuery(Name="filter2")] string? filter2
    ) 
    {
      // Retrieve the list of processes here
      return new ResponseDto 
      { 
          Ok = true,
          ProcList =
          [
              new ResponseProccessDto { Id = 123, Status = "active", CreatedAt = "2023-10-01T12:00:00Z" },
              new ResponseProccessDto { Id = 124, Status = "completed", CreatedAt = "2023-09-30T12:00:00Z" }
          ]
      };
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseDto
    {
        [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]                 
        public required bool Ok { get; set; }
        
        [JsonPropertyName("proc-list")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]                 
        public ResponseProccessDto[]? ProcList { get; set; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseProccessDto
    {      
        [JsonPropertyName("id")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]       
        public long Id { get; set; }

        [JsonPropertyName("status")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]       
        public string? Status { get; set; }

        [JsonPropertyName("created-at")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? CreatedAt { get; set; }
    }

}

