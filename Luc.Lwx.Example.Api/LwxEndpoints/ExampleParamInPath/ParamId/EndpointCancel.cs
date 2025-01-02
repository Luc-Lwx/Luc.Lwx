using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;
using Microsoft.AspNetCore.Mvc;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleParamInPath.ParamId;

public static partial class EndpointCancel
{ 
    [LwxEndpoint(
      Path = "DELETE /apimanager-prefix/example-param-in-path/{id}/cancel",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "Cancel Example Process",
      SwaggerFuncSummary = "Cancels the example process",
      SwaggerFuncDescription = "This endpoint cancels the example process using the provided parameters.",
      SwaggerGroupTitle = "Example Process (Old Way - Not Recommended)",
      LowMaintanability_ParameterInPath_Justification="This is an example. Although not recommended, someone may need this to implement old style endpoints."
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Cancel the example process"
    )]
    public async static Task<ResponseDto> Execute
    ( 
      HttpContext ctx,
      [FromRoute(Name="id")] int id
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