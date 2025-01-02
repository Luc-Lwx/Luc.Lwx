using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleParamInPath.ParamId;

public static partial class EndpointStep1
{ 
    [LwxEndpoint(
      Path = "PUT /apimanager-prefix/example-param-in-path/{id}/step1",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Execute Step 1 of Example Process",
      SwaggerFuncSummary = "Executes the first step of the example process",
      SwaggerFuncDescription = "This endpoint executes the first step of the example process using the provided parameters.",
      SwaggerGroupTitle = "Example Process (Old Way - Not Recommended)",
      LowMaintanability_ParameterInPath_Justification="This is an example. Although not recommended, someone may need this to implement old style endpoints."
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Step 1 of the example process"
    )]
    public async static Task<ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromRoute(Name="id")] int id,
      [FromBody] RequestDto request
    ) 
    {
      // Process the request here
      return new ResponseDto { Ok = true };
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class RequestDto
    {
        [JsonPropertyName("data")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Data { get; set; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public class ResponseDto
    {
        [JsonPropertyName("ok")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Ok { get; set; }
    }
}