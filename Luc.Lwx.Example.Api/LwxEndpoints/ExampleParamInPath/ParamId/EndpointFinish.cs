using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Example.Api.Model;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleParamInPath.ParamId;

public static partial class EndpointFinish
{ 
    [LwxEndpoint(
      Path = "PUT /apimanager-prefix/example-param-in-path/{id}/finish",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Finish Example Process",
      SwaggerFuncSummary = "Finishes the example process",
      SwaggerFuncDescription = "This endpoint finishes the example process using the provided parameters.",
      SwaggerGroupTitle = "Example Process (Old Way - Not Recommended)",
      LowMaintanability_ParameterInPath_Justification="This is an example. Although not recommended, someone may need this to implement old style endpoints."
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Finish the example process"
    )]
    public async static Task<ExampleParamInPathFinishResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromRoute(Name="id")] int id,
      [FromBody] ExampleParamInPathFinishRequestDto request
    ) 
    {
      // Process the request here
      return new ExampleParamInPathFinishResponseDto { Ok = true };
    }
}