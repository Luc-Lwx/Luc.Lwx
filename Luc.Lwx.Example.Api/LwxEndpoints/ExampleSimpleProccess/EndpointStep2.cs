using System.Diagnostics.CodeAnalysis;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;

public static partial class EndpointStep2
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/step2",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Example Step 2",
      SwaggerFuncSummary = "Executes the second step of the example process",
      SwaggerFuncDescription = "This endpoint handles the execution of the second step in the example process.",
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Executes step 2 of the example process"
    )]
    public async static Task<Step2ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] Step2RequestDto request
    ) 
    {
      // Process the request here
      return new Step2ResponseDto { Ok = true };
    }
}
