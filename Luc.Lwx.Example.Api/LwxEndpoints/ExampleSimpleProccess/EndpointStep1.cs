using System.Diagnostics.CodeAnalysis;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Example.Api.Model;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleSimpleProccess;



public static partial class EndpointStep1
{ 
    [LwxEndpoint(
      Path = "POST /apimanager-prefix/example-simple-proccess/step1",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Example Step 1",
      SwaggerFuncSummary = "Executes the first step of the example process",
      SwaggerFuncDescription = "This endpoint handles the execution of the first step in the example process.",
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Executes step 1 of the example process"
    )]
    public async static Task<Step1ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromQuery(Name="proc_id")] decimal proc_id,
      [FromBody] Step1RequestDto request
    ) 
    {
      // Process the request here
      return new Step1ResponseDto { Ok = true };
    }

    public class Step1RequestDto
    {
        public decimal Cde { get; set; }
    }

    public class Step1ResponseDto
    {
        public bool Ok { get; set; }
    }
}
