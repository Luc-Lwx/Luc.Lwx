using System.Diagnostics.CodeAnalysis;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Example.Api.Model;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointCancel
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/cancel",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Cancel Example Process",
      SwaggerFuncSummary = "Cancels the example process",
      SwaggerFuncDescription = "This endpoint handles the cancellation of the example process.",
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Cancels the example process"
    )]
    public async static Task<EndpointCancelRequestDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] CancelRequestDto request
    ) 
    {
      // Cancel the process here
      return new EndpointCancelRequestDto { Ok = true };
    }


    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class EndpointCancelRequestDto
    {
        public bool Ok { get; set; }
    }
    
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class CancelRequestDto
    {
        public bool AreYouSure { get; set; }
    }
}

